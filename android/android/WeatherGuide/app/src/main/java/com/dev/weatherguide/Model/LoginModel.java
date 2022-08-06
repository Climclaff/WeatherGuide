package com.dev.weatherguide.Model;

public class LoginModel {
    public String UserName;
    public String Password;
    public LoginModel(){

    }
    public LoginModel(String name, String password){
        UserName = name;
        Password = password;
    }
}
