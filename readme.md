# UnitNumbers
A C# library to work with numbers with unit.

# Documentation
## Types:
### Dimension:
This is a struct to store the dimension of a number. For example feet, meter, and mile all are of the length dimension type. Area(km^2, m^2, and ...) has the dimension of Length^2. The `Length` parameter of the `Dimension` struct shows the power of the length dimension:

  feet, meter, mile, ...   -> `Dimension.Length=1`
  
  square feet, square meter, square mile, ...   -> `Dimension.Length=2`

This struct has 7 properties(for seven base units). These properties are: `Mass`,`Length`,`Time`,`Temperature`,`Current`,`Mole`, and `Luminosity`. They are all initialized to 0.

### Dimensions: 
A struct containing static properties for easier initialization of dimensions. For example instead of this:
```csharp
new Dimension(){Length=3, Time=-1};//flow rate dimension
```
you can use:
```csharp
Dimensions.FlowRate;
```
### Unit:
This type represent a unit. e.g. **ft**, **meter**, ...
To create a new unit:
```csharp
var hour = new Unit("hr", Dimensions.Time, 3600.0, 0.0);
```

### UnitNumber:
This type represent the actual number with unit. e.g. **1 ft**

## Getting started

### Defining a new unit:

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
Unit ft = new Unit("ft", Dimensions.Length, 0.3048);
// defining the Centigrade(C) unit. It is of the temperature type. And K = 1*C + 273.15
Unit c = new Unit("C", Dimensions.Temperature, 1,273.15);
// defining the Farenheit(F) unit. It is of the temperature type. And K = 5.0/9.0*F + (273.15-32.0/1.8)
Unit f = new Unit("F", Dimensions.Temperature, 5.0/9.0,273.15-32.0/1.8);
```

You can also define units by multiplying and dividing other units, or raising them to some powers:

```csharp
Unit ft = new Unit("ft", Dimensions.Length, 0.3048);
Unit min = new Unit("min", Dimension.Time, 60.0);
Unit ft2 = ft*ft; //defining square foot
Unit ft3 = ft.Pow(3);//defining cubic foot
Unit CFM = ft3/min;
```

Basic units created by constructor are called Basic units. You can find out if a unit is basic by `IsBasic` property. This property returns `true` for basic units and `false` for units that are created using mathematical notations. This is important for units that have offset from each others. For example `C^2/C` has the temperature^1 dimension. `K^2/K` also has the temperature dimension, but `C^2/C` should be converted to `K^2/K` using following formula:

```
1 C^2/C = 1 K^2/K
```

not the `1 C^2/C +273.15 = 1 K^2/K`. `IsBasic` provides a way to distinguish between `C^2/C` and `C`.

### Unit conversion:
suppose we defined these units:
```csharp
Unit ft = new Unit("ft", Dimensions.Length, 0.3048);
Unit ft2 = ft*ft; //defining square foot
Unit ft3 = ft.Pow(3);//defining cubic foot
Unit cm = new Unit("cm", Dimensions.Length, 0.01);
Unit cm2 = cm.Pow(2);
Unit cm3 = cm.Pow(3);
```
You can convert from and to SI units(of the same dimension) using unit itself:
```csharp
double l1 = ft.FromSI(1.2);//1.2ft to meter
double a1 = cm2.ToSI(5.6);//5.6cm^2 to meter^2
```
if you have a unit number you can get its value in any other unit as long as they have the same dimensions:
```csharp
var n1 = new UnitNumber(1.2,ft);
var n2 = new UnitNumber(5.4,cm2);
double d1 = n1.GetValue(cm);//getting n1 in cm unit
double d1 = n1.GetValue(cm2);//Error: An error is thrown! because dimensions wont match.
```

