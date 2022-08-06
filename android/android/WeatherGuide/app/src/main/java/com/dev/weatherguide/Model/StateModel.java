package com.dev.weatherguide.Model;

public class StateModel {
    public int Id;
    public String Name;

    public StateModel(int id, String name){
        Id = id;
        Name = name;
    }

    public StateModel() {

    }

    @Override
    public String toString() {
        return this.Name; // What to display in the Spinner list.
    }
}
