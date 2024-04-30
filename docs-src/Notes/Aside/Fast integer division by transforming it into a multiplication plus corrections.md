# Fast integer division by transforming it into a multiplication plus corrections

Inspired by https://www.eevblog.com/forum/fpga/dividemod-a-32-bit-value-by-1000/ + Hackers Delight book.

For more precise / correct info, see Hackers Delight, 2nd edition, chapter 10.



## Re: Divide/Mod a 32-bit value by 1000

« **Reply #16 on:** July 04, 2021, 06:19:37 pm »

In case you're still interested, here's a small script to factor any divisor.  

Code:

```
#! /usr/bin/env python3   
import math as m   
import sys      
WANT=(int)(sys.argv[1])      
first=round(m.log(WANT,2))   
error=1./WANT - 1./(2**first)   
terms=[first]      
while m.fabs(error) > 1./(1<<63):     
  sign = m.copysign(1, error)     
  term = -round(m.log(m.fabs(error), 2.))     
  terms.append(sign*term)     
  error -= sign*(2**(-term))      
  print(terms)   
```

And if you run it for 1000,  

Code: 

```
[10, 15.0, -17.0, 21.0, 24.0, 26.0, -29.0, -33.0, -34.0, 36.0, -38.0, -42.0, 45.0, -48.0, -50.0, -51.0, 53.0, -60.0]   
```

This says  

Code: 

```
x/1000 ~= (x >> 10)     
    + (x >> 15)     
    - (x >> 17)     
    + (x >> 21)     
    + (x >> 24)     
    + ...   
```

Obviously, for a 16-bit integer, terms past 1>>15 don't matter.  For 32-bit integers, the largest term is -(x >> 29).  
  
A cursory check shows this yields a 1-bit error for values 1000-65535.  Noticeable though is that 1000-1023 yield 0 for 16-bit integers.  
  
(BTW, it's probably not accurate to the full 64 bits due to lack of precision in the 64-bit floating point error variable.  I think it's something like 53 bits.)  
  
Edit: here's a variant to output a C++ function for up to 32 bit integers.  

Code:

```
#! /usr/bin/env python3   
import math as m   
import sys      
WANT=(int)(sys.argv[1])      
first=round(m.log(WANT,2))   
error=1./WANT - 1./(2**first)   
terms=[first]      
print("template <typename T>")   
print("static inline T div_%s(const T& val) {" % WANT)   
print("  return (val >> %s)" % first, end='')      
while m.fabs(error) > 1./(1<<32):
  sign = m.copysign(1, error)
  term = -round(m.log(m.fabs(error), 2.))     
  print(" %s (val >> %s)" % ("-" if sign == -1.0 else "+", (int)(term)), end='')
  error -= sign*(2**(-term))      
print(";\n}")   
```  

Which produces:  
  
Code:

```
template <typename T>   
static inline T div_1000(const T& val) {     
  return (val >> 10) + (val >> 15) - (val >> 17) + (val >> 21) 
       + (val >> 24) + (val >> 26) - (val >> 29);   
}   
```

A #pragma to suppress warnings about integer shifts always being zero might be useful for integers smaller than uint32_t.


