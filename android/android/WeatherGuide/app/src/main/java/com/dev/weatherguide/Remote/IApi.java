package com.dev.weatherguide.Remote;
import com.dev.weatherguide.Model.ChangeNameModel;
import com.dev.weatherguide.Model.ChangePasswordModel;
import com.dev.weatherguide.Model.CountryModel;
import com.dev.weatherguide.Model.LoginModel;
import com.dev.weatherguide.Model.LoginResponse;
import com.dev.weatherguide.Model.RegisterModel;
import com.dev.weatherguide.Model.RegisterResponse;
import com.dev.weatherguide.Model.StateModel;
import com.dev.weatherguide.Model.UsrModel;

import java.util.List;

import io.reactivex.Observable;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.POST;
import retrofit2.http.Query;
import retrofit2.http.QueryName;

public interface IApi {
    //https://localhost:5001/
    @POST("api/ApiAuth/Login")
    Call<LoginResponse> login(@Body LoginModel model);

    @POST("api/ApiAuth/Register")
    Call<RegisterResponse> register(@Body RegisterModel model);

    @POST("api/ApiAuth/AutoLogin")
    Call<LoginResponse> autoLogin(@Header("Authorization") String token);

    @POST("api/ApiData/ChangeLocation")
    Call<String> changeLocation(@Header("Authorization") String token, @Query("countryid") int countryid, @Query("stateid") int stateid);

    @POST("api/ApiData/GeolocationService")
    Call<String> geolocationService(@Header("Authorization") String token);

    @POST("api/ApiData/ChangeName")
    Call<String> changeName(@Header("Authorization") String token, @Body ChangeNameModel model);

    @POST("api/ApiData/ChangePassword")
    Call<String> changePassword(@Header("Authorization") String token, @Body ChangePasswordModel model);




    @GET("api/ApiData/Country")
    Call<String> listCountries();

    @GET("api/ApiData/State")
    Call<String> listStates(@Query("id") int id);

    @GET("api/ApiData/ProfileCountry")
    Call<String> profileCountry(@Header("Authorization") String token);

    @GET("api/ApiData/ProfileState")
    Call<String> profileState(@Header("Authorization") String token);

    @GET("api/ApiData/UserInfo")
    Call<String> userInfo(@Header("Authorization") String token);

    @GET("api/ApiData/Recommendation")
    Call<String> getRecommendation(@Header("Authorization") String token);
}
