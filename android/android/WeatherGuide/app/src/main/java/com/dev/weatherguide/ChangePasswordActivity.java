package com.dev.weatherguide;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
import android.content.SharedPreferences;
import android.content.res.Resources;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import com.dev.weatherguide.Model.ChangePasswordModel;
import com.dev.weatherguide.Remote.IApi;
import com.dev.weatherguide.Remote.RetrofitClient;
import com.google.android.material.snackbar.Snackbar;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ChangePasswordActivity extends AppCompatActivity {
    IApi iWeatherApi;
    Context context;
    Resources resources;
    EditText edt_newPassword, edt_repeatPassword, edt_oldPassword;
    Button btn_change_password, btn_back;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_change_password);
        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(ChangePasswordActivity.this);
        String lang = preferences.getString("Locale.Helper.Selected.Language","uk");
        context = LocaleHelper.setLocale(ChangePasswordActivity.this,lang);
        resources =context.getResources();
        iWeatherApi = RetrofitClient.getInstance().create(IApi.class);

        edt_newPassword = (EditText) findViewById(R.id.changePassword_new_edt);
        edt_repeatPassword = (EditText) findViewById(R.id.changePassword_repeat_edt);
        edt_oldPassword = (EditText) findViewById(R.id.changePassword_old_edt);
        btn_change_password = (Button) findViewById(R.id.changePassword_confirm_button);
        btn_back = (Button) findViewById(R.id.changePassword_back_btn);

        edt_oldPassword.setHint(resources.getString(R.string.old_password_hint));
        edt_newPassword.setHint(resources.getString(R.string.new_password_hint));
        edt_repeatPassword.setHint(resources.getString(R.string.new_password_repeat_hint));
        btn_back.setText(resources.getString(R.string.btn_back));
        btn_change_password.setText(resources.getString(R.string.change_password));


        btn_change_password.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                ChangePasswordModel model = new ChangePasswordModel(edt_oldPassword.getText().toString(),
                        edt_newPassword.getText().toString(), edt_repeatPassword.getText().toString());
                if (model.getNewPassword().equals("") || model.getOldPassword().equals("") || model.getConfirmPassword().equals("")){
                    Snackbar.make(view, resources.getString(R.string.empty_fields), 2000).show();
                    return;
                }
                if (model.getOldPassword().equals(model.getNewPassword())){
                    Snackbar.make(view, resources.getString(R.string.new_password_equal_to_old), 2000).show();
                    return;
                }
                if (!model.getNewPassword().equals(model.getConfirmPassword())){
                    Snackbar.make(view, resources.getString(R.string.passwords_not_equal), 2000).show();
                    return;
                }
                if (!model.getNewPassword().matches("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!_@#&()–[{}]:;',?/*~$^+=<>]).{8,20}$")){
                    Snackbar.make(view,  resources.getString(R.string.new_password_weak), 2000).show();
                    return;
                }
                SharedPreferences prefs;
                prefs= getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
                String token = prefs.getString("token","");

                Call<String> call = iWeatherApi.changePassword(token, model);
                call.enqueue(new Callback<String>() {
                    @Override
                    public void onResponse(Call<String> call,
                                           Response<String> response) {
                        if(response.isSuccessful()){
                            Snackbar.make(view, resources.getString(R.string.password_changed), 2000).show();
                        }
                        if (response.code() == 401 || response.code() == 403){
                                Snackbar.make(view, resources.getString(R.string.jwt_expired), 2000).show();
                        }
                        if (response.code() == 429) {
                            Snackbar.make(view, resources.getString(R.string.too_many_requests), 2000).show();
                        }
                        else {
                            Snackbar.make(view, resources.getString(R.string.old_password_wrong), 2000).show();
                        }

                    }

                    @Override
                    public void onFailure(Call<String> call, Throwable t) {

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