# UnitNumbers
A C# library to work with numbers with unit.

# Documentation
## Types:
### Dimension:
This is a struct to store the dimension of a number. For example feet, meter, and mile all are of the length dimension type. Area(km^2, m^2, and ...) has the dimesion of Length^2. The `Length` parameter of the `Dimension` struct shows the power of the length dimension:

  feet, meter, mile, ...   -> `Dimension.Length=1`
  
  square feet, square meter, square mile, ...   -> `Dimension.Length=2`

This struct has 7 properties(for seven base units). These properties are: `Mass`,`Length`,`Time`,`Temperature`,`Current`,`Mole`, and `Luminosity`. They are all initialized to 0.

### Unit:
This type represent a unit. e.g. **ft**, **meter**, ...
To create a new unit:


### UnitNumber:
This type represent the actual number with unit. e.g. **1 ft**

## Getting started

Defining a new unit:

The constructor is:

```csharp
public Unit(string identifier, Dimension dimension, double multiplier, double sum = 0.0)
```

* Identifier: an identifier for the unit for example cm for centimeter
* Dimension: Dimension of the unit: for example Length=1 (Length) for meter and Length=2 (Length^2) for square meter(area)
* multiplier: to what number should this unit multiplied to convert it to the SI unit. for example cm should be multiplied to 0.01 to convert it to meter(which is SI standard unit for length) 1cm=0.01m
* sum: What number should be added to this unit to convert it to the SI unit. for example we should add 273.15 to °C to convert it to Kelvin(which is SI standard unit for temperature) 

```csharp
// defining the foot(ft) unit. It is of the Length type. And one foot equals
// 0.3048 m (meter is the SI unit for length) 
Unit ft = new Unit("ft", new Dimension() { Length = 1 }, 0.3048);
// defining the Centigrade(C) unit. It is of the temperature type. And K = 1*C + 273.15
Unit c = new Unit("C", new Dimension() {Temperature = 1}, 1,273.15);
// defining the Farenheit(F) unit. It is of the temperature type. And K = 5.0/9.0*F + (273.15-32.0/1.8)
Unit f = new Unit("F", new Dimension() {Temperature = 1}, 5.0/9.0,273.15-32.0/1.8);
```

You can also define units by multiplying and dividing other units, or raising them to some powers:

```csharp
Unit ft = new Unit("ft", new Dimension() { Length = 1 }, 0.3048);
Unit min = new Unit("min", new Dimension() { Time = 1 }, 60.0);
Unit ft2 = ft*ft; //defining square foot
Unit ft3 = ft.Pow(3);//defining cubic foot
Unit CFM = ft3/min;
```

Basic units created by constructor are called Basic units. You can find out if a unit is basic by `IsBasic` property. This property returns `true` for basic units and `false` for units that are created using mathematical notations. This is important for units that have offset from each others. For example `C^2/C` has the temperature^1 dimension. `K^2/K` also has the temperature dimension, but `C^2/C` should be converted to `K^2/K` using following formula:

```
1 C^2/C = 1 K^2/K
```

not the `1 C^2/C +273.15 = 1 K^2/K`. `IsBasic` provides a way to distinguish between `C^2/C` and `C`.