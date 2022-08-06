package com.dev.weatherguide.Model;

public class ChangePasswordModel {
    public String OldPassword ;
    public String NewPassword ;
    public String ConfirmPassword;
    public ChangePasswordModel(){

    }
    public ChangePasswordModel(String oldPassword, String newPassword, String confirmPassword){
        OldPassword = oldPassword;
        NewPassword = newPassword;
        ConfirmPassword = confirmPassword;
    }
    public String getConfirmPassword() {
        return ConfirmPassword;
    }

    public String getNewPassword() {
        return NewPassword;
    }

    public String getOldPassword() {
        return OldPassword;
    }

    public void setConfirmPassword(String confirmPassword) {
        ConfirmPassword = confirmPassword;
    }

    public void setNewPassword(String newPassword) {
        NewPassword = newPassword;
    }

    public void setOldPassword(String oldPassword) {
        OldPassword = oldPassword;
    }
}
