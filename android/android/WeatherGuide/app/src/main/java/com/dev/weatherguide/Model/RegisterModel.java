package com.dev.weatherguide.Model;

public class RegisterModel {
    public String Email;

    public String UserName;

    public String Password ;

    public String Name;

    public String Surname;

    public int CountryId ;

    public int StateId ;

    public RegisterModel(){}
    public RegisterModel(String email, String userName, String password, String name,
                         String surname, int countryId, int stateId){
        Email = email;
        UserName = userName;
        Password = password;
        Name = name;
        Surname = surname;
        CountryId = countryId;
        StateId = stateId;
    }
}

