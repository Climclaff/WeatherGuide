package com.dev.weatherguide.Model;

public class CountryModel {
    public int Id;
    public String Name;

    public CountryModel(int id, String name){
        Id = id;
        Name = name;
    }

    public CountryModel() {

    }


    @Override
    public String toString() {
        return this.Name; // What to display in the Spinner list.
    }
}
