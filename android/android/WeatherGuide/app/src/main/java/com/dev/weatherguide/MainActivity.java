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
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.dev.weatherguide.Model.LoginModel;
import com.dev.weatherguide.Model.LoginResponse;
import com.dev.weatherguide.Model.UsrModel;
import com.dev.weatherguide.Remote.IApi;
import com.dev.weatherguide.Remote.RetrofitClient;
import com.google.android.material.snackbar.Snackbar;

import dmax.dialog.SpotsDialog;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class MainActivity extends AppCompatActivity {
    IApi iWeatherApi;
    Context context;
    Resources resources;
    EditText edt_email, edt_password;
    Button btn_login, btn_toRegister, btn_locale;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        iWeatherApi = RetrofitClient.getInstance().create(IApi.class);
        edt_email = (EditText) findViewById(R.id.edt_email);
        edt_password = (EditText)findViewById(R.id.edt_password);
        btn_login = (Button) findViewById(R.id.btn_login);
        btn_toRegister = (Button) findViewById(R.id.btn_toRegister);
        btn_locale = (Button)findViewById(R.id.button_change_locale);

        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(MainActivity.this);
        String lang = preferences.getString("Locale.Helper.Selected.Language","uk");
        context = LocaleHelper.setLocale(MainActivity.this,lang);
        resources =context.getResources();


        edt_email.setHint(resources.getString(R.string.email_hint));
        edt_password.setHint(resources.getString(R.string.password_hint));
        btn_login.setText(resources.getString(R.string.login));
        btn_toRegister.setText(resources.getString(R.string.btn_register));
        btn_locale.setText(resources.getString(R.string.language));

        // Auto Login
        SharedPreferences prefs;
        prefs=getSharedPreferences("myPrefs",Context.MODE_PRIVATE);
        String token = prefs.getString("token","");
        String user = prefs.getString("user","");
        Toast.makeText(MainActivity.this, user, Toast.LENGTH_SHORT).show();
        if(!token.equals("") && !user.equals("")){
            Call<LoginResponse> call = iWeatherApi.autoLogin(token);
            call.enqueue(new Callback<LoginResponse> (){
                @Override
                public void onResponse( Call<LoginResponse> call,
                                        Response<LoginResponse> response) {
                    if (response.isSuccessful()) {
                        SharedPreferences prefs;
                        SharedPreferences.Editor edit;
                        prefs=getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
                        edit= prefs.edit();
                        String user=response.body().getUser();
                        edit.putString("user",user);
                        edit.commit();
                        Intent intent = new Intent(MainActivity.this, AuthenticatedActivity.class);
                        startActivity(intent);
                    }else {
                        Toast.makeText(MainActivity.this, "response.body().getToken()", Toast.LENGTH_SHORT).show();
                    }
                }
                @Override
                public void onFailure(Call<LoginResponse> call,Throwable t) {
                    Toast.makeText(MainActivity.this, "Failed to connect", Toast.LENGTH_SHORT).show();
                    t.printStackTrace();
                }
            });
        }
        //

        btn_locale.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View view){
                Intent intent = new Intent(MainActivity.this, ChangeLanguageActivity.class);
                startActivity(intent);
            }
        });
        btn_toRegister.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View view){
                Intent intent = new Intent(MainActivity.this, RegisterActivity.class);
                startActivity(intent);
            }
        });
        btn_login.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                AlertDialog dialog = new SpotsDialog.Builder()
                        .setContext(MainActivity.this)
                        .build();
                dialog.show();
                LoginModel user = new LoginModel(edt_email.getText().toString(),
                        edt_password.getText().toString());

                Call<LoginResponse> call = iWeatherApi.login(user);
                call.enqueue(new Callback<LoginResponse> (){
                    @Override
                    public void onResponse( Call<LoginResponse> call,
                                            Response<LoginResponse> response) {
                        // Save token to SharedPreferences then authorize every request with bearer token in header
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
                            Intent intent = new Intent(MainActivity.this, AuthenticatedActivity.class);
                            startActivity(intent);
                        } else {
                            Snackbar.make(view, resources.getString(R.string.login_wrong), 2000).show();
                        }
                        dialog.dismiss();
                    }
                    @Override
                    public void onFailure(Call<LoginResponse> call,Throwable t) {
                        dialog.dismiss();
                        t.printStackTrace();
                    }
                });

            }
        });
    }

    @Override
    public void onResume(){
        super.onResume();
        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(MainActivity.this);
        String lang = preferences.getString("Locale.Helper.Selected.Language","uk");
        context = LocaleHelper.setLocale(MainActivity.this,lang);
        resources =context.getResources();
        edt_email.setHint(resources.getString(R.string.email_hint));
        edt_password.setHint(resources.getString(R.string.password_hint));
        btn_login.setText(resources.getString(R.string.login));
        btn_toRegister.setText(resources.getString(R.string.btn_register));
        btn_locale.setText(resources.getString(R.string.language));
    }
}
