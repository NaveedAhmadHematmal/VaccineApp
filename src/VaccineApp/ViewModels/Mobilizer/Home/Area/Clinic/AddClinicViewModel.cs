﻿using VaccineApp.Features;
using Core.Models;
using DAL.Persistence;
using System.Windows.Input;
using VaccineApp.ViewModels.Base;

namespace VaccineApp.ViewModels.Mobilizer.Home.Area.Clinic;

public class AddClinicViewModel : ViewModelBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IToast _toast;
    private ClinicModel _clinic;
    private bool _isLocationAvailable;

    ClinicValidator _clinicValidator { get; set; }


    public AddClinicViewModel(UnitOfWork unitOfWork, ClinicModel clinic, IToast toast)
    {
        _unitOfWork = unitOfWork;
        _toast = toast;
        _clinic = clinic;
        _clinicValidator = new();

        PostCommand = new Command(Post);
    }

    public ICommand PostCommand { private set; get; }

    private async void Post()
    {
        var validationResult = _clinicValidator.Validate(Clinic);
        if (validationResult.IsValid)
        {
            if (IsLocationAvailable)
            {
                try
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();

                    if (location != null)
                    {
                        Clinic.Latitude = location.Latitude.ToString();
                        Clinic.Longitude = location.Longitude.ToString();
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
            var result = await _unitOfWork.AddClinic(Clinic);
            await Shell.Current.GoToAsync("../");
            _toast.MakeToast($"{result.ClinicName} has been added");
        }
        else
        {
            _toast.MakeToast(validationResult.Errors[0].PropertyName, validationResult.Errors[0].ErrorMessage);
        }
    }

    public bool IsLocationAvailable
    {
        get { return _isLocationAvailable; }
        set { _isLocationAvailable = value; OnPropertyChanged(); }
    }
    public ClinicModel Clinic
    {
        get
        {
            return _clinic;
        }
        set
        {
            _clinic = value;
            OnPropertyChanged();
        }
    }
}
