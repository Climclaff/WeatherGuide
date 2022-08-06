package com.dev.weatherguide;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.Fragment;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.res.Resources;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;

import com.google.android.material.bottomnavigation.BottomNavigationMenu;
import com.google.android.material.bottomnavigation.BottomNavigationMenuView;
import com.google.android.material.bottomnavigation.BottomNavigationView;

public class AuthenticatedActivity extends AppCompatActivity implements BottomNavigationView.OnNavigationItemSelectedListener {
    BottomNavigationView bottomNavigationView;
    Context context;
    Resources resources;
    int currentScreen;
    Fragment selectedFragment;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_authenticated);
        bottomNavigationView = findViewById(R.id.bottomNavigationView);
        bottomNavigationView.setOnNavigationItemSelectedListener(this);
        bottomNavigationView.setSelectedItemId(R.id.home);

        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(AuthenticatedActivity.this);
        String lang = preferences.getString("Locale.Helper.Selected.Language","uk");
        context = LocaleHelper.setLocale(AuthenticatedActivity.this,lang);
        resources =context.getResources();

        bottomNavigationView.getMenu().findItem(R.id.home).setTitle(resources.getString(R.string.home));
        bottomNavigationView.getMenu().findItem(R.id.recommendations).setTitle(resources.getString(R.string.recommendations));
        bottomNavigationView.getMenu().findItem(R.id.profile).setTitle(resources.getString(R.string.profile));

    }
    HomeFragment homeFragment = new HomeFragment();
    RecommendationsFragment recommendationsFragment = new RecommendationsFragment();
    ProfileFragment profileFragment = new ProfileFragment();



    @Override
    public boolean onNavigationItemSelected(@NonNull MenuItem item) {

        switch (item.getItemId()) {
            case R.id.home:
                getSupportFragmentManager().beginTransaction().replace(R.id.flFragment, homeFragment).commit();
                currentScreen = R.id.home;
                selectedFragment = homeFragment;
                return true;

            case R.id.recommendations:
                getSupportFragmentManager().beginTransaction().replace(R.id.flFragment, recommendationsFragment).commit();
                currentScreen = R.id.recommendations;
                selectedFragment = recommendationsFragment;
                return true;

            case R.id.profile:
                getSupportFragmentManager().beginTransaction().replace(R.id.flFragment, profileFragment).commit();
                currentScreen = R.id.profile;
                selectedFragment = profileFragment;
                return true;
        }
        return false;
    }

    @Override
    public void onSaveInstanceState(Bundle savedInstanceState) {
        super.onSaveInstanceState(savedInstanceState);
        savedInstanceState.putInt("currentScreen", currentScreen);
    }
    @Override
    public void onRestoreInstanceState(Bundle savedInstanceState){
        BottomNavigationView f = (BottomNavigationView) findViewById(R.id.bottomNavigationView);
        f.setSelectedItemId(savedInstanceState.getInt("currentScreen"));
        super.onRestoreInstanceState(savedInstanceState);

    }
}