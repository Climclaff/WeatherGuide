package com.dev.weatherguide;

import androidx.appcompat.app.AppCompatActivity;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.res.Resources;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.RelativeLayout;
import android.widget.TextView;

public class ChangeLanguageActivity extends AppCompatActivity {

    TextView helloworld,dialog_language;
    int lang_selected;
    RelativeLayout show_lan_dialog;
    Context context;
    Resources resources;
    Button btn_back;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_change_language);
        dialog_language = (TextView)findViewById(R.id.dialog_language);
        helloworld =(TextView)findViewById(R.id.text_choose_lang);
        show_lan_dialog = (RelativeLayout)findViewById(R.id.showlangdialog);
        btn_back = (Button) findViewById(R.id.button_locale_back);

        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(ChangeLanguageActivity.this);
        String lang = preferences.getString("Locale.Helper.Selected.Language","uk");
        context = LocaleHelper.setLocale(ChangeLanguageActivity.this,lang);
        resources =context.getResources();


        btn_back.setText(resources.getString(R.string.btn_back));
        btn_back.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View view){
                finish();
            }
        });
        if(LocaleHelper.getLanguage(ChangeLanguageActivity.this).equalsIgnoreCase("en"))
        {
            context = LocaleHelper.setLocale(ChangeLanguageActivity.this,"en");
            resources =context.getResources();
            dialog_language.setText("en");
            helloworld.setText(resources.getString(R.string.text_choose_lang));
            setTitle(resources.getString(R.string.app_name));
            lang_selected = 0;
        }else if(LocaleHelper.getLanguage(ChangeLanguageActivity.this).equalsIgnoreCase("uk")){
            context = LocaleHelper.setLocale(ChangeLanguageActivity.this,"uk");
            resources =context.getResources();
            dialog_language.setText("uk");
            helloworld.setText(resources.getString(R.string.text_choose_lang));
            setTitle(resources.getString(R.string.app_name));
            lang_selected =1;
        }

        show_lan_dialog.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                final String[] Language = {"en","uk"};
                final int checkItem;
                Log.d("Clicked","Clicked");
                final AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(ChangeLanguageActivity.this);
                SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(ChangeLanguageActivity.this);
                String lang = preferences.getString("Locale.Helper.Selected.Language","uk");
                context = LocaleHelper.setLocale(ChangeLanguageActivity.this,lang);
                resources =context.getResources();
                dialogBuilder.setTitle(resources.getString(R.string.text_choose_lang))
                        .setSingleChoiceItems(Language, lang_selected, new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialogInterface, int i) {
                                dialog_language.setText(Language[i]);
                                if(Language[i].equals("en")){
                                    context = LocaleHelper.setLocale(ChangeLanguageActivity.this,"en");
                                    resources =context.getResources();
                                    lang_selected = 0;
                                    helloworld.setText(resources.getString(R.string.text_choose_lang));
                                    dialogBuilder.setTitle(resources.getString(R.string.text_choose_lang));
                                    setTitle(resources.getString(R.string.app_name));
                                }
                                if(Language[i].equals("uk"))
                                {
                                    context = LocaleHelper.setLocale(ChangeLanguageActivity.this,"uk");
                                    resources =context.getResources();
                                    lang_selected = 1;
                                    helloworld.setText(resources.getString(R.string.text_choose_lang));
                                    dialogBuilder.setTitle(resources.getString(R.string.text_choose_lang));
                                    setTitle(resources.getString(R.string.app_name));
                                }
                            }
                        })
                        .setPositiveButton("OK", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialogInterface, int i) {
                                SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(ChangeLanguageActivity.this);
                                String lang = preferences.getString("Locale.Helper.Selected.Language","uk");
                                context = LocaleHelper.setLocale(ChangeLanguageActivity.this,lang);
                                resources =context.getResources();
                                btn_back.setText(resources.getString(R.string.btn_back));
                                dialogInterface.dismiss();
                            }
                        });
                dialogBuilder.create().show();
            }
        });
    }
    }
