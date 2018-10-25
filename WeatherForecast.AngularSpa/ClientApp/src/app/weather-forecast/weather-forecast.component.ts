import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-weather-forecast',
  templateUrl: './weather-forecast.component.html',
  styleUrls: ['./weather-forecast.component.css']
})
export class WeatherForecastComponent {
  public forecast: WeatherForecast;
  public currentUnit: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    let getWeatherForecastUrl = baseUrl + 'api/Weather/GetWeatherForecast/';

    let selectedCity = localStorage.getItem("City");
    let selectedUnit = localStorage.getItem("Unit");

    selectedCity = !selectedCity ? "Kharkiv" : selectedCity;

    if(selectedCity){
      getWeatherForecastUrl += "?name=" + selectedCity;
    }

    if(selectedUnit){
      getWeatherForecastUrl += "&unit=" + selectedUnit;      
      this.setUnitToDispay();
    }

    http.get<WeatherForecast>(getWeatherForecastUrl).subscribe(result => {
      this.forecast = result;
    }, error => console.error(error));
  }

  public toLocalTime(unixTime: number) {
    let date = new Date(unixTime * 1000);
    return date.toLocaleTimeString();
  }

  public setUnitToDispay() {
    let selectedUnit = localStorage.getItem("Unit");

    switch(selectedUnit.toLocaleLowerCase()){
      case "metric": {
        this.currentUnit = "°С";
        break;
      }
      case "imperial": {
        this.currentUnit = "°F";
        break;
      }
      case "kelvin": {
        this.currentUnit = "°K";
        break;
      }
    }
  }
}

interface WeatherForecast {
  place: string;
  weatherDescription: string;
  imageUrl: string;
  country: string;
  sunrise: number;
  sunset: number;
  countryImage: string;
  icon: string;
  temperature: number;
  windSpeed: number;
  overcast: number;
  pressure: number;
  humidity: number;
}
