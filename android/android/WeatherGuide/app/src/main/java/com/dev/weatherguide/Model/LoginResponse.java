package com.dev.weatherguide.Model;

import java.util.Date;

public class LoginResponse {
    String token;
    String user;


    public String getToken() {
        return token;
    }

    public String getUser() {
        return user;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public void setUser(String user) {
        this.user = user;
    }
}
