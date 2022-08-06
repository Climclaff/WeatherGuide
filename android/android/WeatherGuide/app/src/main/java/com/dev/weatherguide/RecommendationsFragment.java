package com.dev.weatherguide;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.SharedPreferences;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import android.preference.PreferenceManager;
import android.util.Base64;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import com.dev.weatherguide.Model.StateModel;
import com.dev.weatherguide.Remote.IApi;
import com.dev.weatherguide.Remote.RetrofitClient;
import com.google.android.material.snackbar.Snackbar;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.text.DecimalFormat;
import java.text.SimpleDateFormat;
import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.Date;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.TimeUnit;

import dmax.dialog.SpotsDialog;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;


public class RecommendationsFragment extends Fragment {
    public RecommendationsFragment(){

    }
    IApi iWeatherApi;
    Context context;
    Resources resources;
    Activity referenceActivity;
    SharedPreferences preferences;
    String FirstClothingName;
    String SecondClothingName;
    String ThirdClothingName;
    byte[] firstClothingImage;
    byte[] secondClothingImage;
    byte[] thirdClothingImage;
    String temperature;
    String windSpeed;
    String humidity;
    Call<String> call = null;
    TextView weatherText, recommendationText, humidityText, windSpeedText, temperatureText, firstClothingText, secondClothingText, thirdClothingText;
    ImageView firstClothingImageView, secondClothingImageView, thirdClothingImageView;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_recommendations, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        iWeatherApi = RetrofitClient.getInstance().create(IApi.class);
        referenceActivity = getActivity();
        preferences = PreferenceManager.getDefaultSharedPreferences(getContext());
        String lang = preferences.getString("Locale.Helper.Selected.Language","uk");
        context = LocaleHelper.setLocale(getContext(),lang);
        resources =context.getResources();

        weatherText = (TextView) getView().findViewById(R.id.text_recommendation_weather);
        recommendationText = (TextView) getView().findViewById(R.id.text_recommendation_today);
        humidityText = (TextView) getView().findViewById(R.id.text_recommendation_humidity);
        windSpeedText = (TextView) getView().findViewById(R.id.text_recommendation_windspeed);
        temperatureText = (TextView) getView().findViewById(R.id.text_recommendation_temperature);
        firstClothingText = (TextView) getView().findViewById(R.id.text_recommendation_firstclothing);
        secondClothingText = (TextView) getView().findViewById(R.id.text_recommendation_secondclothing);
        thirdClothingText = (TextView) getView().findViewById(R.id.text_recommendation_thirdclothing);
        firstClothingImageView = (ImageView) getView().findViewById(R.id.image_recommendation_firstclothing);
        secondClothingImageView = (ImageView) getView().findViewById(R.id.image_recommendation_secondclothing);
        thirdClothingImageView = (ImageView) getView().findViewById(R.id.image_recommendation_thirdclothing);

        boolean isCacheLoaded = loadRecommendationFromCache();
        if(!isCacheLoaded) {
            loadRecommendation();
        }





    }

    @Override
    public void onDestroyView () {
        super.onDestroyView();
        if(call != null) {
            call.cancel();
        }

    }

    private boolean loadRecommendationFromCache(){
        SimpleDateFormat formatter = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");
        Date currentDate = new Date();
        long difference;
        SharedPreferences prefs;
        prefs = referenceActivity.getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
        String token = prefs.getString("token", "");
        String cachedForToken = prefs.getString("cachedForToken", "");
        if(token.equals(cachedForToken)) {
            String cachingDate = prefs.getString("recommendationCacheTime", "");
            if (cachingDate != null) {
                try {
                    Date cachedDate = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss").parse(cachingDate);
                    long diffInMillies = Math.abs(cachedDate.getTime() - currentDate.getTime());
                    difference = TimeUnit.SECONDS.convert(diffInMillies, TimeUnit.MILLISECONDS);
                } catch (Exception ex) {
                    return false;
                }
                if (difference > 0 && difference <= 900) {
                    String humidity = prefs.getString("humidityText", "");
                    String windSpeed = prefs.getString("windSpeedText", "");
                    String temperature = prefs.getString("temperatureText", "");
                    String firstClothingName = prefs.getString("firstClothingText", "");
                    String secondClothingName = prefs.getString("secondClothingText", "");
                    String thirdClothingName = prefs.getString("thirdClothingText", "");
                    Bitmap firstClothingImageBitmap = stringToBitmap(prefs.getString("firstClothingImageView", ""));
                    Bitmap secondClothingImageBitmap = stringToBitmap(prefs.getString("secondClothingImageView", ""));
                    Bitmap thirdClothingImageBitmap = stringToBitmap(prefs.getString("thirdClothingImageView", ""));


                    weatherText.setText(resources.getString(R.string.weather_inregion));
                    recommendationText.setText(resources.getString(R.string.recommendation_today));
                    humidityText.setText(humidity);
                    windSpeedText.setText(windSpeed);
                    temperatureText.setText(temperature);
                    firstClothingText.setText(firstClothingName);
                    secondClothingText.setText(secondClothingName);
                    thirdClothingText.setText(thirdClothingName);
                    firstClothingImageView.setImageBitmap(Bitmap.createScaledBitmap(firstClothingImageBitmap,
                            200, 200, false));
                    secondClothingImageView.setImageBitmap(Bitmap.createScaledBitmap(secondClothingImageBitmap,
                            200, 200, false));
                    thirdClothingImageView.setImageBitmap(Bitmap.createScaledBitmap(thirdClothingImageBitmap,
                            200, 200, false));
                    return true;
                } else {
                    return false;
                }
            }
        }
        return false;
    }
    private void putDataToCache(Bitmap firstImg, Bitmap secondImg, Bitmap thirdImg){
        SharedPreferences prefs;
        SharedPreferences.Editor edit;
        prefs = referenceActivity.getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
        edit = prefs.edit();
        edit.putString("humidityText", humidityText.getText().toString());
        edit.putString("windSpeedText", windSpeedText.getText().toString());
        edit.putString("temperatureText", temperatureText.getText().toString());
        edit.putString("firstClothingText", firstClothingText.getText().toString());
        edit.putString("secondClothingText", secondClothingText.getText().toString());
        edit.putString("thirdClothingText", thirdClothingText.getText().toString());
        edit.putString("firstClothingImageView", bitmapToString(firstImg));
        edit.putString("secondClothingImageView", bitmapToString(secondImg));
        edit.putString("thirdClothingImageView", bitmapToString(thirdImg));
        edit.putString("cachedForToken", prefs.getString("token",""));
        SimpleDateFormat formatter = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");
        Date date = new Date();
        edit.putString("recommendationCacheTime", formatter.format(date));
        edit.commit();
    }
    private void loadRecommendation(){
            SharedPreferences prefs;
            prefs= referenceActivity.getSharedPreferences("myPrefs", Context.MODE_PRIVATE);
            String token = prefs.getString("token","");
            call = iWeatherApi.getRecommendation(token);
            call.enqueue(new Callback<String>(){
                @Override
                public void onResponse( Call<String> call,
                                        Response<String> response) {
                    if(response.isSuccessful()) {
                        String jsonresponse = response.body().toString();
                        try {
                            JSONObject obj = new JSONObject(jsonresponse);
                            JSONObject measurement = obj.getJSONObject("measurement");
                            if (preferences.getString("Locale.Helper.Selected.Language", "uk").equals("en")){
                                FirstClothingName = obj.getString("firstClothingNameEN");
                                SecondClothingName = obj.getString("secondClothingNameEN");
                                ThirdClothingName = obj.getString("thirdClothingNameEN");
                                double convertedTemp = ((double) measurement.getInt("temperature"));
                                DecimalFormat format = new DecimalFormat("0.#");
                                temperature =String.valueOf(format.format((convertedTemp*1.8)+32))+" °F";
                                windSpeed = String.valueOf(measurement.getInt("windSpeed")*2)+" mp/h";
                            } else {
                                FirstClothingName = obj.getString("firstClothingNameUA");
                                SecondClothingName = obj.getString("secondClothingNameUA");
                                ThirdClothingName = obj.getString("thirdClothingNameUA");
                                temperature =String.valueOf(measurement.getInt("temperature"))+" °C";
                                windSpeed = String.valueOf(measurement.getInt("windSpeed"))+" м/с";
                            }
                            firstClothingImage = Base64.decode(obj.getString("firstClothingImage"),Base64.DEFAULT);
                            secondClothingImage = Base64.decode(obj.getString("secondClothingImage"),Base64.DEFAULT);
                            thirdClothingImage = Base64.decode(obj.getString("thirdClothingImage"),Base64.DEFAULT);
                            humidity = String.valueOf(measurement.getInt("humidity")+" %");
                            weatherText.setText(resources.getString(R.string.weather_inregion));
                            recommendationText.setText(resources.getString(R.string.recommendation_today));
                            humidityText.setText(resources.getString(R.string.humidity)+" "+humidity);
                            windSpeedText.setText(resources.getString(R.string.windspeed)+" "+windSpeed);
                            temperatureText.setText(resources.getString(R.string.temperature)+" "+temperature);
                            firstClothingText.setText(FirstClothingName);
                            secondClothingText.setText(SecondClothingName);
                            thirdClothingText.setText(ThirdClothingName);
                            Bitmap firstImg = BitmapFactory.decodeByteArray(firstClothingImage, 0, firstClothingImage.length);
                            Bitmap secondImg = BitmapFactory.decodeByteArray(secondClothingImage, 0, secondClothingImage.length);
                            Bitmap thirdImg = BitmapFactory.decodeByteArray(thirdClothingImage, 0, thirdClothingImage.length);
                            firstClothingImageView.setImageBitmap(Bitmap.createScaledBitmap(firstImg,
                                    200, 200, false));
                            secondClothingImageView.setImageBitmap(Bitmap.createScaledBitmap(secondImg,
                                    200, 200, false));
                            thirdClothingImageView.setImageBitmap(Bitmap.createScaledBitmap(thirdImg,
                                    200, 200, false));

                            putDataToCache(firstImg, secondImg, thirdImg);
                        } catch (JSONException e) {
                            e.printStackTrace();

                        }
                    }
                    if (response.code() == 401 || response.code() == 403){
                        Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.jwt_expired), 2000).show();
                    }
                    if (response.code() == 429) {
                        Snackbar.make(getActivity().findViewById(android.R.id.content), resources.getString(R.string.too_many_requests), 2000).show();
                    }

                }
                @Override
                public void onFailure(Call<String> call,Throwable t) {
                    t.printStackTrace();
                }
            });
    }
    private String bitmapToString(Bitmap bitmap){
        ByteArrayOutputStream baos=new  ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.PNG,100, baos);
        byte [] b=baos.toByteArray();
        String temp= Base64.encodeToString(b, Base64.DEFAULT);
        return temp;
    }
    private Bitmap stringToBitmap(String encodedString){
        try {
            byte [] encodeByte=Base64.decode(encodedString,Base64.DEFAULT);
            Bitmap bitmap= BitmapFactory.decodeByteArray(encodeByte, 0, encodeByte.length);
            return bitmap;
        } catch(Exception e) {
            e.getMessage();
            return null;
        }
    }
}


