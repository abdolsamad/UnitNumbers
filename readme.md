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
