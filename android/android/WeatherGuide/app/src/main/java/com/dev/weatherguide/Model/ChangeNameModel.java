package com.dev.weatherguide.Model;

public class ChangeNameModel {
    String Name;
    String Surname;
    public ChangeNameModel(){

    }
   public ChangeNameModel(String name, String surname){
        Name = name;
        Surname = surname;
    }

    public String getName() {
        return Name;
    }

    public String getSurname() {
        return Surname;
    }

    public void setToken(String name) {
        Name = name;
    }

    public void setUser(String surname) {
        Surname = surname;
    }
}
