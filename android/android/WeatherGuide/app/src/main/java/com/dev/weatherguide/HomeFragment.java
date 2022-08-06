package com.dev.weatherguide;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;
import android.content.res.Resources;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import android.preference.PreferenceManager;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.TextView;

import com.dev.weatherguide.Remote.IApi;
import com.dev.weatherguide.Remote.RetrofitClient;

public class HomeFragment extends Fragment {
    public HomeFragment(){

    }
    Context context;
    Resources resources;
    Activity referenceActivity;
    SharedPreferences preferences;
    TextView text_home;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        return inflater.inflate(R.layout.fragment_home, container, false);
    }
    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        referenceActivity = getActivity();
        preferences = PreferenceManager.getDefaultSharedPreferences(getContext());
        String lang = preferences.getString("Locale.Helper.Selected.Language","uk");
        context = LocaleHelper.setLocale(getContext(),lang);
        resources =context.getResources();
        text_home = (TextView) getView().findViewById(R.id.text_home);
        text_home.setText(resources.getString(R.string.text_home));

    }
}