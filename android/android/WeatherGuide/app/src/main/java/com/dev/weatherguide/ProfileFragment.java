package com.dev.weatherguide;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.res.Resources;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import android.preference.PreferenceManager;
import android.util.Log;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.SimpleCursorAdapter;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import com.dev.weatherguide.Model.ChangeNameModel;
import com.dev.weatherguide.Model.CountryModel;
import com.dev.weatherguide.Model.StateModel;
import com.dev.weatherguide.Remote.IApi;
import com.dev.weatherguide.Remote.RetrofitClient;
import com.google.android.material.snackbar.Snackbar;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;


public class ProfileFragment extends Fragment {

    public ProfileFragment(){

    }
    Context context;
    Resources resources;
    Activity referenceActivity;
    IApi iWeatherApi;
    List<CountryModel> countries = new ArrayList<CountryModel>();
    List<StateModel> states = new ArrayList<StateModel>();
    List<Integer> countryIDs = new ArrayList<Integer>();
    List<Integer> stateIDs = new ArrayList<Integer>();
    Spinner spinner_country;
    Spinner spinner_state;
    Button btn_geolocation;
    EditText name_edit;
    EditText surname_edit;
    Call<String> call;
    int selectedStateId;
    int selectedCountryId;
    boolean loaded = false;
    boolean locationChanges = false;
    Button to_password, btn_saveLocation, btn_saveName;
    public void setSelectedCountryId(int selectedCountryId) {
        this.selectedCountryId = selectedCountryId;
    }
    public void setSelectedStateId(int selectedStateId) {
        this.selectedStateId = selectedStateId;
    }
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        loaded = false;
        locationChanges = false;
        View root = inflater.inflate(R.layout.fragment_profile, container,false);
        if(countries.size() != 0){
            countries.clear();
        }
        if(states.size() != 0){
            states.clear();
        }
        if(countryIDs.size() != 0){
            countryIDs.clear();
        }
        if(stateIDs.size() != 0){
            stateIDs.clear();
        }
        return root;
    }
    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        referenceActivity = getActivity();
        iWeatherApi = RetrofitClient.getInstance().create(IApi.class);
        Button btn_signout = (Button) getView().findViewById(R.id.profile_signout_button);
        btn_saveLocation = (Button) getView().findViewById(R.id.profile_saveLocation_button);
        btn_saveName = (Button) getView().findViewById(R.id.profile_changeName_button);
        btn_geolocation = (Button) getView().findViewById(R.id.profie_geolocationService_button);
        spinner_country = (Spinner) getView().findViewById(R.id.profile_country_spinner);
        spinner_state = (Spinner) getView().findViewById(R.id.profile_state_spinner);
        name_edit = (EditText) getView().findViewById(R.id.profile_name_editText);
        surname_edit = (EditText) getView().findViewById(R.id.profile_surname_editText);
        to_password = (Button) getView().findViewById(R.id.profile_password_button);
        TextView profile_header = (TextView) getView().findViewById(R.id.profile_header_textView);
        TextView location = (TextView) getView().findViewById(R.id.profile_location_textView);


        loadSpinners();
        loadUserData();

        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(referenceActivity);
        String lang = preferences.getString("Locale.Helper.Selected.Language","uk");
        context = LocaleHelper.setLocale(referenceActivity,lang);
        resources =context.getResources();

        btn_signout.setText(resources.getString(R.string.signout));
        btn_saveLocation.setText(resources.getString(R.string.save));
        btn_saveName.setText(resources.getString(R.string.save));
        to_password.setText(resources.getString(R.string.password));
        name_edit.setHint(resources.getString(R.string.name_hint));
        surname_edit.setHint(resources.getString(R.string.surname_hint));
        profile_header.setText(resources.getString(R.string.profile_header));
        location.setText(resources.getString(R.string.change_location_text));
        btn_geolocation.setText(resources.getString(R.string.geolocation_service));





        to_password.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(referenceActivity, ChangePasswordActivity.class);
                startActivity(intent);
            }
        });
        btn_saveName.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                SharedPreferences prefs;
                prefs= referenceActivity.getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
                String token = prefs.getString("token","");
                ChangeNameModel model = new ChangeNameModel(name_edit.getText().toString(),
                        surname_edit.getText().toString());
                if(model.getSurname().equals("") || model.getName().equals("")){
                    Snackbar.make(view, resources.getString(R.string.empty_fields), 2000).show();
                    return;
                }
                if (model.getName().matches("^(([A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+[\\s]{1}[A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+)" +
                        "|([A-Za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+))$") &&
                        model.getSurname().matches("^(([A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+[\\s]{1}[A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+)" +
                        "|([A-Za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+))$")) {
                    Call<String> call = iWeatherApi.changeName(token, model);
                    call.enqueue(new Callback<String>() {
                        @Override
                        public void onResponse(Call<String> call,
                                               Response<String> response) {
                            if (response.code() == 401 || response.code() == 403){
                                Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.jwt_expired), 2000).show();
                                disableButtons();
                            }
                            if (response.code() == 429) {
                                Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.too_many_requests), 2000).show();
                            }

                        }

                        @Override
                        public void onFailure(Call<String> call, Throwable t) {

                            t.printStackTrace();
                        }
                    });
                    Snackbar.make(view, resources.getString(R.string.info_changed), 2000).show();
                } else{
                    Snackbar.make(view, resources.getString(R.string.name_wrong)+" "+resources.getString(R.string.or_text)+" "+resources.getString(R.string.surname_wrong), 2000).show();
                }
            }
        });
        btn_signout.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                SharedPreferences prefs;
                SharedPreferences.Editor edit;
                prefs = referenceActivity.getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
                edit = prefs.edit();
                edit.putString("user", "");
                edit.putString("token", "");
                edit.commit();
                referenceActivity.finish();
            }
        });
        spinner_country.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                if (loaded == true) {
                    CountryModel country = (CountryModel) parent.getSelectedItem();
                    Call<String> call = iWeatherApi.listStates(country.Id);
                    call.enqueue(new Callback<String>() {
                        @Override
                        public void onResponse(Call<String> call,
                                               Response<String> response) {

                            String jsonresponse = response.body().toString();
                            try {
                                if (states.size() > 0) {
                                    states.clear();
                                }
                                JSONObject obj = new JSONObject(jsonresponse);
                                JSONArray dataArray = obj.getJSONArray("data");
                                if (stateIDs.size() != 0)
                                    stateIDs.clear();
                                for (int i = 0; i < dataArray.length(); ++i) {
                                    StateModel stateModel = new StateModel();
                                    JSONObject dataobj = dataArray.getJSONObject(i);
                                    stateModel.Name = dataobj.getString("name");
                                    stateModel.Id = dataobj.getInt("id");
                                    states.add(stateModel);
                                    stateIDs.add(stateModel.Id);
                                }
                                ArrayAdapter<StateModel> stateAdapter = new ArrayAdapter<>(getActivity().createDeviceProtectedStorageContext(), R.layout.support_simple_spinner_dropdown_item, states);
                                spinner_state.setAdapter(stateAdapter);
                                StateModel state = (StateModel) spinner_state.getSelectedItem();
                                setSelectedStateId(state.Id);
                                setSelectedCountryId(country.Id);
                            } catch (JSONException e) {
                                e.printStackTrace();
                            }
                        }

                        @Override
                        public void onFailure(Call<String> call, Throwable t) {
                            t.printStackTrace();
                        }
                    });
                }
                loaded = true;
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });
        btn_saveLocation.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                SharedPreferences prefs;
                prefs = referenceActivity.getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
                String token = prefs.getString("token", "");

                if (locationChanges) {
                    Call<String> call = iWeatherApi.changeLocation(token, selectedCountryId, selectedStateId);
                    call.enqueue(new Callback<String>() {
                        @Override
                        public void onResponse(Call<String> call,
                                               Response<String> response) {
                            if(response.isSuccessful()){
                                Snackbar.make( getActivity().findViewById(android.R.id.content), resources.getString(R.string.location_changed),2000).show();
                            }
                            if (response.code() == 420) {
                                Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.recommendation_cooldown), 2000).show();
                            }
                            if (response.code() == 401 || response.code() == 403) {
                                Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.jwt_expired), 2000).show();
                                disableButtons();
                            }
                            if (response.code() == 429) {
                                Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.too_many_requests), 2000).show();
                            }
                        }

                        @Override
                        public void onFailure(Call<String> call, Throwable t) {

                            t.printStackTrace();
                        }
                    });
                } else {
                Snackbar.make( getActivity().findViewById(android.R.id.content), resources.getString(R.string.no_changes),2000).show();
            }
            locationChanges = false;
            }
        });
        btn_geolocation.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                SharedPreferences prefs;
                prefs = referenceActivity.getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
                String token = prefs.getString("token", "");

                    Call<String> call = iWeatherApi.geolocationService(token);
                    call.enqueue(new Callback<String>() {
                        @Override
                        public void onResponse(Call<String> call,
                                               Response<String> response) {
                            if(response.isSuccessful()){
                                Snackbar.make( getActivity().findViewById(android.R.id.content), resources.getString(R.string.geolocation_service_success),2000).show();
                                loadSpinners();
                            }
                            if (response.code() == 420) {
                                Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.recommendation_cooldown), 2000).show();
                            }
                            if (response.code() == 401 || response.code() == 403) {
                                Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.jwt_expired), 2000).show();
                                disableButtons();
                            }
                            if (response.code() == 429) {
                                Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.too_many_requests), 2000).show();
                            }
                            if (response.code() == 400) {
                                Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.geolocation_service_fail), 2000).show();
                            }

                        }

                        @Override
                        public void onFailure(Call<String> call, Throwable t) {

                            t.printStackTrace();
                        }
                    });

            }
        });
        spinner_state.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {

            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                StateModel obj = ((StateModel) spinner_state.getSelectedItem());
                selectedStateId = obj.Id;
                locationChanges = true;
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {

            }
        });
    }
    @Override
    public void onDestroyView () {
        super.onDestroyView();
        if(call != null) {
            call.cancel();
        }

    }

    private void loadSpinners(){
        SharedPreferences prefs;
        prefs= referenceActivity.getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
        String token = prefs.getString("token","");
        call = iWeatherApi.profileCountry(token);
        call.enqueue(new Callback<String>(){
            @Override
            public void onResponse( Call<String> call,
                                    Response<String> response) {
                if(response.isSuccessful()) {
                    String jsonresponse = response.body().toString();
                    try {
                        JSONObject obj = new JSONObject(jsonresponse);
                        JSONArray dataArray = obj.getJSONArray("data");
                        for (int i = 0; i < dataArray.length(); ++i) {
                            CountryModel countryModel = new CountryModel();
                            JSONObject dataobj = dataArray.getJSONObject(i);
                            countryModel.Name = dataobj.getString("name");
                            countryModel.Id = dataobj.getInt("id");
                            countries.add(countryModel);
                            countryIDs.add(countryModel.Id);
                        }
                        JSONObject responseCountry = obj.getJSONObject("country");
                        selectedCountryId = responseCountry.getInt("id");
                        CountryModel country = new CountryModel();
                        country.Name = responseCountry.getString("name");
                        country.Id = selectedCountryId;
                        ArrayAdapter<CountryModel> countryAdapter = new ArrayAdapter<>(getActivity().createDeviceProtectedStorageContext(), R.layout.support_simple_spinner_dropdown_item, countries);
                        spinner_country.setAdapter(countryAdapter);
                        selectSpinnerItemByValue(spinner_country,countryIDs.indexOf(country.Id));
                        loadStates(country.Id);
                        locationChanges = false;

                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                }


            }
            @Override
            public void onFailure(Call<String> call,Throwable t) {
                t.printStackTrace();
            }
        });
    }
    private void loadStates(int countryId){
        SharedPreferences prefs;
        prefs= referenceActivity.getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
        String token = prefs.getString("token","");
        String user = prefs.getString("user","");
        Call<String> call   = iWeatherApi.profileState(token);
        call.enqueue(new Callback<String> (){
            @Override
            public void onResponse( Call<String> call,
                                    Response<String> response) {
                if(response.isSuccessful()) {
                    String jsonresponse = response.body().toString();
                    try {
                        if (states.size() > 0) {
                            states.clear();
                        }
                        JSONObject obj = new JSONObject(jsonresponse);
                        JSONArray dataArray = obj.getJSONArray("data");
                        if (stateIDs.size() > 0)
                            stateIDs.clear();
                        for (int i = 0; i < dataArray.length(); ++i) {
                            StateModel stateModel = new StateModel();
                            JSONObject dataobj = dataArray.getJSONObject(i);
                            stateModel.Name = dataobj.getString("name");
                            stateModel.Id = dataobj.getInt("id");
                            states.add(stateModel);
                            stateIDs.add(stateModel.Id);
                        }
                        JSONObject responseState = obj.getJSONObject("state");
                        StateModel state = new StateModel();
                        state.Name = responseState.getString("name");
                        state.Id = responseState.getInt("id");
                        ArrayAdapter<StateModel> stateAdapter = new ArrayAdapter<>(getActivity().createDeviceProtectedStorageContext(), R.layout.support_simple_spinner_dropdown_item, states);
                        spinner_state.setAdapter(stateAdapter);
                        setSelectedStateId(state.Id);
                        setSelectedCountryId(countryId);
                        selectSpinnerItemByValue(spinner_state, stateIDs.indexOf(state.Id));
                        locationChanges = false;
                    } catch (JSONException e) {
                        e.printStackTrace();

                    }
                }

            }
            @Override
            public void onFailure(Call<String> call,Throwable t) {
                t.printStackTrace();
            }
        });

    }
    private void loadUserData(){
        SharedPreferences prefs;
        prefs= referenceActivity.getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
        String token = prefs.getString("token","");
        Call<String> call = iWeatherApi.userInfo(token);
        call.enqueue(new Callback<String>(){
            @Override
            public void onResponse( Call<String> call, Response<String> response) {
                if(response.isSuccessful()) {
                    String jsonresponse = response.body().toString();
                    try {
                        JSONObject obj = new JSONObject(jsonresponse);
                        String responseName = obj.getString("name");
                        String responseSurname = obj.getString("surname");
                        name_edit.setText(responseName);
                        surname_edit.setText(responseSurname);
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                }
                if (response.code() == 401 || response.code() == 403){
                    Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.jwt_expired), 2000).show();
                    disableButtons();
                }

            }
            @Override
            public void onFailure(Call<String> call,Throwable t) {
                t.printStackTrace();
            }
        });
    }

    public static void selectSpinnerItemByValue(Spinner spnr, long value) {
        ArrayAdapter adapter     = (ArrayAdapter) spnr.getAdapter();
        for (int position = 0; position < adapter.getCount(); position++) {
            if(adapter.getItemId(position) == value) {
                spnr.setSelection(position);
                return;
            }
        }
    }
    private void disableButtons(){
        btn_saveLocation.setEnabled(false);
        btn_saveName.setEnabled(false);
        to_password.setEnabled(false);
    }

}