package com.dev.weatherguide;

import androidx.appcompat.app.AppCompatActivity;

import android.app.AlertDialog;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.res.Resources;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.util.Log;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import com.dev.weatherguide.Model.CountryModel;
import com.dev.weatherguide.Model.LoginModel;
import com.dev.weatherguide.Model.LoginResponse;
import com.dev.weatherguide.Model.RegisterModel;
import com.dev.weatherguide.Model.RegisterResponse;
import com.dev.weatherguide.Model.StateModel;
import com.dev.weatherguide.Remote.IApi;
import com.dev.weatherguide.Remote.RetrofitClient;
import com.google.android.material.snackbar.Snackbar;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

import dmax.dialog.SpotsDialog;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class RegisterActivity extends AppCompatActivity {
    IApi iWeatherApi;
    Context context;
    Resources resources;
    TextView txt_title;
    EditText edt_email, edt_password, edt_name, edt_surname;
    Spinner edt_country, edt_state;
    Button btn_register, btn_back;
    int selectedCountryId = 0;
    int selectedStateId = 0;
    List<Integer> stateIDs = new ArrayList<Integer>();
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_register);
        if(stateIDs.size() != 0){
            stateIDs.clear();
        }
        iWeatherApi = RetrofitClient.getInstance().create(IApi.class);

        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(RegisterActivity.this);
        String lang = preferences.getString("Locale.Helper.Selected.Language","uk");
        context = LocaleHelper.setLocale(RegisterActivity.this,lang);
        resources =context.getResources();

        txt_title = (TextView) findViewById(R.id.reg_title);
        edt_email = (EditText) findViewById(R.id.edt_reg_email);
        edt_password = (EditText) findViewById(R.id.edt_reg_password);
        edt_name = (EditText) findViewById(R.id.edt_reg_name);
        edt_surname = (EditText) findViewById(R.id.edt_reg_surname);
        edt_country = (Spinner) findViewById(R.id.edt_reg_country_spinner);
        edt_state = (Spinner) findViewById(R.id.edt_reg_state_spinner);
        btn_register = (Button) findViewById(R.id.btn_register);
        btn_back = (Button) findViewById(R.id.reg_button_back);
        List<CountryModel> countries = new ArrayList<CountryModel>();
        List<StateModel> states = new ArrayList<StateModel>();

        edt_email.setHint(resources.getString(R.string.email_hint));
        edt_name.setHint(resources.getString(R.string.name_hint));
        edt_surname.setHint(resources.getString(R.string.surname_hint));
        edt_password.setHint(resources.getString(R.string.password_hint));
        btn_back.setText(resources.getString(R.string.btn_back));
        btn_register.setText(resources.getString(R.string.btn_register));
        txt_title.setText(resources.getString(R.string.reg_title));

        Call<String> call = iWeatherApi.listCountries();
        call.enqueue(new Callback<String> (){
            @Override
            public void onResponse( Call<String> call,
                                    Response<String> response) {

                String jsonresponse = response.body().toString();
                try {
                    JSONObject obj = new JSONObject(jsonresponse);
                    JSONArray dataArray  =  obj.getJSONArray("data");
                    for(int i = 0; i<dataArray.length(); ++i){
                        CountryModel countryModel = new CountryModel();
                        JSONObject dataobj = dataArray.getJSONObject(i);
                        countryModel.Name = dataobj.getString("name");
                        countryModel.Id = dataobj.getInt("id");
                        countries.add(countryModel);
                    }
                    ArrayAdapter<CountryModel> countryAdapter = new ArrayAdapter<>(RegisterActivity.this, R.layout.support_simple_spinner_dropdown_item, countries);
                    edt_country.setAdapter(countryAdapter);

                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }
            @Override
            public void onFailure(Call<String> call,Throwable t) {
                Toast.makeText(RegisterActivity.this, "Failed to connect", Toast.LENGTH_SHORT).show();
                t.printStackTrace();
            }
        });



        edt_state.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                StateModel state = (StateModel) edt_state.getSelectedItem();
                selectedStateId = state.Id;
            }
            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
            });

        edt_country.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                CountryModel country = (CountryModel) parent.getSelectedItem();
                Call<String> call   = iWeatherApi.listStates(country.Id);
                call.enqueue(new Callback<String> (){
                    @Override
                    public void onResponse( Call<String> call,
                                            Response<String> response) {

                        String jsonresponse = response.body().toString();
                        try {
                            if (states.size() > 0){
                                states.clear();
                            }
                            JSONObject obj = new JSONObject(jsonresponse);
                            JSONArray dataArray  =  obj.getJSONArray("data");
                            if (stateIDs.size() != 0)
                                stateIDs.clear();
                            for(int i = 0; i<dataArray.length(); ++i){
                                StateModel stateModel = new StateModel();
                                JSONObject dataobj = dataArray.getJSONObject(i);
                                stateModel.Name = dataobj.getString("name");
                                stateModel.Id = dataobj.getInt("id");
                                states.add(stateModel);
                                stateIDs.add(stateModel.Id);
                            }
                            ArrayAdapter<StateModel> stateAdapter = new ArrayAdapter<>(RegisterActivity.this, R.layout.support_simple_spinner_dropdown_item, states);
                            edt_state.setAdapter(stateAdapter);
                            StateModel state = (StateModel) edt_state.getSelectedItem();
                            selectedStateId = state.Id;
                            selectedCountryId = country.Id;
                        } catch (JSONException e) {
                            e.printStackTrace();
                        }
                    }
                    @Override
                    public void onFailure(Call<String> call,Throwable t) {
                        Toast.makeText(RegisterActivity.this, "Failed to connect", Toast.LENGTH_SHORT).show();
                        t.printStackTrace();
                    }
                });
            }
            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });

        btn_register.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                AlertDialog dialog = new SpotsDialog.Builder()
                        .setContext(RegisterActivity.this)
                        .build();
                dialog.show();
                RegisterModel user = new RegisterModel(
                        edt_email.getText().toString(),
                        edt_email.getText().toString(),
                        edt_password.getText().toString(),
                        edt_name.getText().toString(),
                        edt_surname.getText().toString(),
                        selectedCountryId,
                        selectedStateId);


                if(!user.UserName.matches("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")){
                    Snackbar.make(view, resources.getString(R.string.email_wrong), 2000).show();
                    dialog.dismiss();
                    return;
                }
                if(!user.Password.matches("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!_@#&()–[{}]:;',?/*~$^+=<>]).{8,20}$")){
                    Snackbar.make(view, resources.getString(R.string.new_password_weak), 2000).show();
                    dialog.dismiss();
                    return;
                }
                if (!user.Name.matches("^(([A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+[\\s]{1}[A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+)" +
                        "|([A-Za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+))$") ||
                        !user.Surname.matches("^(([A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+[\\s]{1}[A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+)" +
                                "|([A-Za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+))$")) {
                    Snackbar.make(view, resources.getString(R.string.name_wrong)+" "+resources.getString(R.string.or_text)+" "+resources.getString(R.string.surname_wrong), 2000).show();
                    dialog.dismiss();
                    return;
                }
                Call<RegisterResponse> call = iWeatherApi.register(user);
                call.enqueue(new Callback<RegisterResponse>() {
                    @Override
                    public void onResponse(Call<RegisterResponse> call,
                                           Response<RegisterResponse> response) {
                        if (response.isSuccessful()) {
                            Call<LoginResponse> loginCall = iWeatherApi.login(new LoginModel(edt_email.getText().toString(),
                                    edt_password.getText().toString()));
                            loginCall.enqueue(new Callback<LoginResponse> (){
                                @Override
                                public void onResponse( Call<LoginResponse> call,
                                                        Response<LoginResponse> response) {
                                    if (response.isSuccessful()) {
                                        SharedPreferences prefs;
                                        SharedPreferences.Editor edit;
                                        prefs=getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
                                        edit= prefs.edit();
                                        String saveToken=response.body().getToken();
                                        String user=response.body().getUser();
                                        edit.putString("user",user);
                                        edit.putString("token","Bearer "+saveToken);
                                        edit.commit();
                                        Intent intent = new Intent(RegisterActivity.this, AuthenticatedActivity.class);
                                        startActivity(intent);
                                    } else {
                                        Snackbar.make(view, "Error", 2000).show();
                                    }
                                }
                                @Override
                                public void onFailure(Call<LoginResponse> call,Throwable t) {
                                    t.printStackTrace();
                                }
                            });

                        } else {
                            Snackbar.make(view, resources.getString(R.string.user_already_exists), 2000).show();
                        }
                        dialog.dismiss();
                    }

                    @Override
                    public void onFailure(Call<RegisterResponse> call, Throwable t) {
                        dialog.dismiss();
                        t.printStackTrace();
                    }
                });

            }

        });

        btn_back.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                finish();
            }
            });
    }

    }