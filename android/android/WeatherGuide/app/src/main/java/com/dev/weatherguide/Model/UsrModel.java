package com.dev.weatherguide.Model;

public class UsrModel {
    String User;
public UsrModel(){

}
   public UsrModel(String name){
    User = name;
    }
    public String getUser() {
        return User;
    }

    public void setUser(String user) {
        User = user;
    }
}
