import { Component } from '@angular/core';

declare var microsoftTeams;

@Component({
  selector: 'app-configuration-component',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.css']
})
export class ConfigurationComponent {
  public cities: NameValue[] = [
    { name: "Atlanta, US", value: "Atlanta" },
    { name: "Barcelona, ES", value: "Barcelona" },
    { name: "Berlin, DE", value: "Berlin" },
    { name: "Kharkiv, UA", value: "Kharkiv" },
    { name: "London, GB", value: "London" },
    { name: "Kyiv, UA", value: "Kyiv" },
    { name: "Lviv, UA", value: "Lviv" },
    { name: "Madrid, ES", value: "Madrid" },
    { name: "Milan, IT", value: "Milan" },
    { name: "Odessa, UA", value: "Odessa" },
    { name: "Toronto, CA", value: "Toronto" }
  ];
  public units: NameValue[] = [
    { name: "Celsius", value: "Metric" },
    { name: "Fahrenheit", value: "Imperial" },
    { name: "Kelvin", value: "Kelvin" }
  ];

  public selectedCity : string;
  public selectedUnit : string;

  constructor() {
    let city = localStorage.getItem("City");
    let unit = localStorage.getItem("Unit");

    this.selectedCity = city ? city : "Kharkiv";
    this.selectedUnit = unit ? unit : "Metric";

    this.applySelected();
    this.initMicrosoftTeams();

    microsoftTeams.settings.setValidityState(true);
  }
  
  public applySelected() {
    localStorage.setItem("City", this.selectedCity);
    localStorage.setItem("Unit", this.selectedUnit);
  }

  private initMicrosoftTeams() {
    microsoftTeams.initialize();

    microsoftTeams.settings.setValidityState(true);
    
    // Save configuration changes
    microsoftTeams.settings.registerOnSaveHandler(function (saveEvent) {
      // Let the Microsoft Teams platform know what you want to load based on
      // what the user configured on this page
      microsoftTeams.settings.setSettings({
        contentUrl: createTabUrl(), // Mandatory parameter
        entityId: createTabUrl() // Mandatory parameter
      });

      // Tells Microsoft Teams platform that we are done saving our settings. Microsoft Teams waits
      // for the app to call this API before it dismisses the dialog. If the wait times out, you will
      // see an error indicating that the configuration settings could not be saved.
      saveEvent.notifySuccess();
    });
    
    // Create the URL that Microsoft Teams will load in the tab. You can compose any URL even with query strings.
    function createTabUrl() {
      return window.location.protocol + '//' + window.location.host + '/weather-forecast';
    }
  }
}

interface NameValue {
  name: string;
  value: string;
}
