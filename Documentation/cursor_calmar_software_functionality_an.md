# Calmar software functionality and documentation
_Exported on 1/12/2026 at 16:08:49 GMT+1 from Cursor (2.2.44)_

---

**User**

Calmar is a software produced many years ago by my company.
It is quite hard to find out how it works.

Calmar Catenary Software.  Trinity House Application.
Calmar is a software tool, as originally supplied, is used for calculating the mooring arrangements for plastic float based navigation buoys dependant on individual station parameters. This document describes how Trinity House has applied this software to their steel buoys.
The software requires a model with attached physical parameters. This can be constructed from up to four parts; float, structure (core), pylon and topmark. These can be sub divided into segments that can each have parameters attached.
After consideration The Trinity Buoy Fig 1 was split to create the Calmar model in   Fig 2. The structure was used to model the steel buoy body and the float was disregarded.
       
		Fig. 1							Fig. 2

Structure:
The Trinity House Type two buoy body was divided into sections, as this means that derivatives of this can be created easily in the software using these and other sections as building blocks. Using CAD modelling tools, the properties required by the Calmar software, weight, and ‘real’ volume of the segments were calculated (Figs. 3 and 4). All brackets, mooring eyes and webbing were included in the section properties. These, along with real dimensions to create the Calmar representations (Fig. 5).
 
			Fig. 3						Fig. 4
       
Fig. 5	
The anchor ring point is positioned where the mooring eyes sit regardless that a bridle mooring arrangement is used.
In the ‘General’ Tab, ballast can be applied to the complete buoy. For the above model, the ballast weights were created as a section and applied to the structure.
Float:
This was rendered inactive by entering zero values.
Pylon and Topmark:
Using representative dimensions the components of the pylon were conveniently grouped and the mass totals established and applied to the representative sections as Fig. 6 and 7
As with the structure, the equipment mass is included in the model segments, and not applied separately with the specific tabs in the software.
         
						Fig. 6
                   
Fig. 7
S. Jacobs 15/10/2013



In this folder you find all the software code.
Are you able to read it and understand how the software works?


CALMAR - TECHNICAL REFERENCES
CALMAR
CATENARY MOORING LINE CALCULATOR
TECHNICAL REFERENCES
1. TABLE OF CONTENTS
1. TABLE OF CONTENTS ............................................................................ 1
2. OBJECT OF PRESENT DOCUMENT ...................................................... 1
3. DOCUMENTATION REFERENCES ......................................................... 1
4. GENERAL CONSIDERATIONS ............................................................... 2
5. LOADS CALCULATION ........................................................................... 3
6. CALCULATION PROCESS ...................................................................... 5
7. CALCULATION PARAMETERS ............................................................... 6
8. RESULTS ................................................................................................. 7
9. DOCUMENTATION .................................................................................. 8
10.
GENERAL NOTES ............................................................................ 8
11.
LIST of ANNEXES ............................................................................. 8
2. OBJECT OF PRESENT DOCUMENT
- Provide technical references of CALMAR, software calculating specifications for
transitional catenary mooring lines for Aids to Navigation (AtoN) in its Release version
0.2.3.0.
3. DOCUMENTATION REFERENCES
[A]IALA Guideline No. 1066 "The Design of Floating Aid to Navigation Moorings"
IALA/AISM - Edition 1.1 (June 2010)
[B]"Recommendation E-107 On the Design of Normal Moorings"
IALA/AISM 1998 (withdrawn),
[C]"Position Mooring" - Offshore Standard
DNV-OS-E301 by Det Norske Veritas (October 2010)
[D] "Environmental Conditions and Environmental Loads"
Recommended Practice RP-C205 by Det Norske Veritas (October 2010)
[E]
"Buoy Engineering " – by Henri O.BERTEAUX
Wiley-Interscience - JOHN WILEY & SONS (1976)
IALA/AISM - CALMAR
27 févr. 2014 - CALMAR-TECHREF-00.PDF – Page 1/8CALMAR - TECHNICAL REFERENCES
4. GENERAL CONSIDERATIONS
4.1
Scope of Calculations
4.1.1 Types of moorings lines
CALMAR calculates characteristics of transitional moorings in perfect catenary regime.
4.1.2 Definition of Catenary Regime
Catenary regime applies when "the mooring chain catenary meets the seabed tangentially
exactly at the sinker when there are the maximum wind and tide (or current) loads on the
buoy. The mooring loads will be transferred horizontally to the sinker which will be working
as effectively as possible" [A].
4.1.3 Composition of moorings lines
CALMAR calculates only characteristics of moorings made of one homogeneous single
type and size chain.
Composite moorings (made of different chain types or diameters with possible portions of
other materials) cannot be calculated in the current version.
4.1.4 Mooring Sites conditions
CALMAR field of reliable calculations, in the actual version, excludes:
- Breaking waves conditions (alarm warning displayed in software - see 6.3.1)
- Environmental loads coming simultaneously from different directions,
- Taut or Slack regime moorings.
Users should also reasonably exclude calculations in following site conditions:
- Deep waters moorings (Depths > 200 m)
- High velocity currents (current velocity > 7 m/s)
4.1.5 Buoys Attitude
CALMAR, in the current version, does not account for buoy attitude (Trim, Roll and Yaw
angles) and therefore the buoy axis is considered vertical.
4.1.6 Buoys Stability
CALMAR, in the current version, does not integrate Vertical Centres of Gravity and
therefore does not account for buoy stability.
4.1.1 Drag Loads
CALMAR, in the current version, only takes into account the static horizontal components
of drag loads.
IALA/AISM - CALMAR
27 févr. 2014 - CALMAR-TECHREF-00.PDF – Page 2/8CALMAR - TECHNICAL REFERENCES
5. LOADS CALCULATION
5.1
Surface Drags Loads in Water
5.1.1 Depth of Water
Calculations in CALMAR are always done for the highest level of Water defined by:
Depth (H) = (Nominal Depth + Tidal Range + (0.5 * Max. Wave Height) in meters.
5.1.2 Dynamic Pressure and Load Formula
All loads are calculated strictly according to IALA Guideline 1066 [A] as:
Drag Load = Dynamic Pressure * Exposed Area * Drag Coefficient / Acceleration of
Gravity
with:
Pd (Dynamic Pressure) = 0.5 * (Medium Velocity ^ 2) * Medium Specific Gravity (Density)
where:
Drag Load
Exposed Lateral Area
Acceleration of Gravity
Media Velocity
Density of media in
in
in
in
in
in
kg
m²
m/s²
m/s
kg/m³
5.1.3 Current Drag on Buoy
Current drag on buoy is calculated as:
Dynamic Pressure * Immersed Buoy and structure Lateral Area * Drag Coefficient
with:
Current speed = Total surface current speed as defined in 5.1.6.
Immersed Buoy Area = Lateral area of immersed portion of buoy and structure at
computed draft.
In order to take into account the usual Surface Roughness and minimum fouling on buoys
and buoy structures, a common drag coefficient of 1.2 is applied.
5.1.4 Surface Wind Generated Current
Speed of surface wind generated current (Vcw) is calculated as:
Vcw = 0.015 * Max Wind Speed (as given by site parameters)
according to Ref [C] & [D].
IALA/AISM - CALMAR
27 févr. 2014 - CALMAR-TECHREF-00.PDF – Page 3/8CALMAR - TECHNICAL REFERENCES
5.1.5 Wave Drag on Buoy
The wave drag on buoy and taken as an additional surface current speed due to wave
particles velocity defined by Morison's load equations and taking in account Wave
Significant Height and Wave Period, both given by site parameters. (see Ref [D]).
5.1.6 Surface Velocity Components
Total surface current speed is defined by adding its separate components:
Speed of Tidal (or Stream) Current (as given by site parameters)
+
Speed of Surface Wind Generated Current (see 5.1.3)
+
Speed equivalent to Wave Drag (see 5.1.3)
=
Total Surface Current Speed
5.2
Drags Loads in Water
5.2.1 Current Drag on Chain
Current drag on buoy chain mooring line is calculated as:
Dynamic Pressure * Chain Area * Drag Coefficient with:
Chain Area = catenary height * 2.65 * chain nominal diameter (according to [C]).
Catenary Height = Depth (see 5.1.1) – calculated Depth of anchor ring under WL
Drag Coefficient = 1.2 to compensate for tangency factor of real catenary length.
5.2.2 Effect of Extensive Fouling
Considering usual regular maintenance schemes of AtoN services, an increase of buoy
displacement and drag caused by extensive fouling (aged of more than two years) has not
been taken into account. (see Ref [D]).
5.2.3 Effects of VIO & VIV
No Drag or Added Mass are taken into account for the effect of VIO (Vortex Induced
Oscillations) or of VIV (Vortex Induced Vibrations) (see Ref [D]).
5.3
Drag Loads in Air
5.3.1 Wind Drag on Buoy
Wind drag on buoy is calculated as:
Dynamic Pressure * Emerged Buoy Area * Drag Coefficient with:
Wind speed = maximum wind speed as given by site parameters.
Emerged Buoy Area = Lateral area of emerged portion of buoy and structure at computed
draft.
In order to take into account the usual surface roughness and minimum fouling on buoys
and buoy structures, a common drag coefficient of 1.2 is applied.
5.3.2 Air Drag on Pylon and Top Mark
Wind drag on Pylon and Top Mark (Superstuctures) is calculated as:
Dynamic Pressure * Lateral Area of Pylon and Top Mark * Drag Coefficient
with:
Wind speed = Maximum wind speed as given by site parameters.
Emerged Superstuctures Area = computed lateral areas of pylon and top mark.
In order to take into account the usual complex structure and construction of pylon and top
mark, a common drag coefficient of 1.2 is applied.
IALA/AISM - CALMAR
27 févr. 2014 - CALMAR-TECHREF-00.PDF – Page 4/8CALMAR - TECHNICAL REFERENCES
6. CALCULATION PROCESS
6.1
Calculation Bases
All catenary characteristics, chain loads, sinker loads and specifications are calculated
strictly according to IALA Guideline 1066 [A].
6.2
Main Calculation Steps
6.2.1 Calculation of Hydrostatic Table for Buoy
A table containing basic Hydrostatic Data (Drafts against Displaced Volume) is calculated
at 1 mm increments from bottom of float to maximum displacement Drafts.
Input volumes are computed as they are defined in model definition.
6.2.2 Initial Buoy Draft Calculation
Initial buoy Displacement and Draft are calculated using declared total buoy mass and
water at its specific gravity.
6.2.3 Initial Drag Loads Calculation
Initial drag load is calculated by addition of different drags:
6.2.3.1 Initial Current Drag on Buoy
Initial current drag on buoy is calculated at initial buoy draft following method described in
5.1.3.
6.2.3.2 Initial Current Drag on Chain
Initial current drag on Chain is calculated at initial buoy draft following method described in
5.2.1.
6.2.3.3 Initial Wind Drag Loads
Initial wind drag load on buoy and superstructures is calculated at initial buoy draft
following method described in 5.3.
6.2.4 Initial Catenary Calculation
Initial catenary length is calculated using initial Drag Load (6.2.3), Depth (5.1.1) and chain
parameters defined by user (A table of chain nominal diameters and mass per length for
all chain types is built in the software).
6.2.5 Iterations for Equilibrium
Buoy displacement is then adjusted in iterations of steps 6.2.2, 6.2.3 and 6.2.4, adding
weight of catenary required to cope with calculated loads and therefore increasing buoy
displacement and drag loads, until the equilibrium solution between final drag loads, final
necessary catenary lifted weight and final buoy displacement is achieved.
IALA/AISM - CALMAR
27 févr. 2014 - CALMAR-TECHREF-00.PDF – Page 5/8CALMAR - TECHNICAL REFERENCES
6.2.6 Calculation of Sinker Minimum Mass
Necessary mass of sinker to compensate for horizontal components of drags is calculated
using sinker specific gravity defined by User, medium specific gravity, computed final
necessary horizontal load, seabed internal friction angle and IALA recommended Safety
Factor (1.5) to calculate minimum sinker mass.
6.3
Other Calculations
6.3.1 Breaking Wave Height
A breaking wave height (Hb) is calculated from User input parameters as:
Hb = (1.56 * (T ^ 2)) * 0.14 If Hm / (1.56 * (T ^ 2)) > 0.14 or otherwise:
Hb = (H * .78)
With:
T = Period of Maximum Height Waves in seconds
H = Maximum Water Depth on station (see 5.1.1)
A warning message is displayed if maximum wave height reaches breaking wave height
and calculations are stopped (see 4.1.4).
7. CALCULATION PARAMETERS
7.1
Units
Defaults units used throughout CALMAR are SI Metric Units.
Flow velocities can be input in m/s (meter per second) or kn (knots)
7.2
Defaults Constant Values
Density of Air
Density of Water
Density of Cast Iron
Density of Steel
Steel Q1 tensile strength
Steel Q2 tensile strength
Steel Q3 tensile strength
Coef. Tensile Strength / Proof Load Studlink Chain
Coef. Tensile Strength / Proof Load 3D Chain
Coef. Tensile Strength / Proof Load 3.5D Chain
Coef. Tensile Strength / Proof Load 4D Chain
Coef. Tensile Strength / Proof Load 5D Chain
Coef. Tensile Strength / Proof Load 9D Chain
Seabed Internal Friction Angle
Sinker Factor Of Safety
Coef. Max Wave Amplitude / Significant Height
Acceleration of Gravity
Buoys Drag Coefficient in Water
Chains Drag Coefficient in Water
Buoys Drag Coefficient in Air
Pylons Drag Coefficient in Air
TopMarks Drag Coefficient in Air
IALA/AISM - CALMAR
a
w
ci
s
K
g
Cdbw
Cdcw
Cdba
Cdpa
Cdta
129
1 025
7 320
7 850
430
500
690
1.414
2
2
2
2
2
45
1.5
1.85
9.81
1.2
1.2
1.2
1.2
1.2
kg/m³
kg/m³
kg/m³
kg/m³
N/mm²
N/mm²
N/mm²
Coef
Coef
Coef
Coef
Coef
Coef
Degrees
Coef
Coef
m/s²
Coef
Coef
Coef
Coef
Coef
27 févr. 2014 - CALMAR-TECHREF-00.PDF – Page 6/8CALMAR - TECHNICAL REFERENCES
8. RESULTS
8.1
Results in Result Screen Table:
8.1.1 Minimum Water Depth
Calculated as (Nominal Depth - (0.5 * Max.Wave Height)) in meters.
8.1.2 Maximum Water Depth
see 5.1.1
8.1.3 Catenary Length
Minimum length of User specified chain to match horizontal drag in catenary regime.
8.1.4 Maximum Swinging Radius
Horizontal distance from sinker mooring eye to buoy's anchor ring under maximum
mooring load.
8.1.5 Maximum Tension in Mooring
Maximum tension applied on mooring line by mooring loads.
8.1.6 Chain Safety Factor
Safety Factor of chain Proof Load divided by maximum tension on chain.
This factor ought to be > to 5 according to IALA Guideline 1066 [A].
8.1.7 Sinker Minimum Mass
see 6.2.6
8.1.8 Horizontal Drag
Total of horizontal components of drag = horizontal component of maximum mooring line
tension.
8.1.9 Buoy Reserve Buoyancy
Portion of total volume of buoy and structure left not used by buoy total displacement.
8.1.10 Buoy Total Displacement
Immersed weight of buoy = Weight of medium displaced by buoy immersed volume.
8.1.11 Buoy Free Board
Vertical distance from Loaded Water Line and top of float (without any trim angle).
8.1.12 Buoy Draft
Vertical distance from deepest part of buoy float or structure and Loaded Water Line
(without any trim angle).
8.1.13 Immersed Lateral Plane
Lateral area of immersed buoy float or structure (without any trim angle) subjected to
current drag loads.
IALA/AISM - CALMAR
27 févr. 2014 - CALMAR-TECHREF-00.PDF – Page 7/8CALMAR - TECHNICAL REFERENCES
8.1.14 Emerged Lateral Plane
Lateral area of total of emerged portions of buoy float or structure plus pylon plus top mark
(without any trim angle) subjected to wind drag loads.
9. DOCUMENTATION
9.1
Acronyms
List of Acronyms used in present document:
AtoN
CALM
Hs
(o.e.)
SPM
VIO
VIV
WL
10.
for
for
for
for
for
for
for
for
Aids to Navigation
Catenary Anchor Legs Mooring
Significant Wave Height
Or Equivalent
Single Point Mooring
Vortex Induced Oscillations
Vortex Induced Vibrations
Water Line
GENERAL NOTES
10.1 Possible Future developments
Following new functions could be added to present version of CALMAR:
- Input of different media Specific Gravity variable,
- Addition of supplementary languages,
- Addition of supplementary chains characteristics,
- Input of seabed internal friction angle variable,
- Calculation of composites moorings,
- Calculation in breaking waves,
- Attitude (trim) of buoy under drag loads,
- Different mooring points variable,
- Approaches on dynamic characteristics,
- Calculation in Ice.
11.
LIST of ANNEXES
No Annex.
XXXXXXXXXXXXXXX END OF DOCUMENT XXXXXXXXXXXXXXX
IALA/AISM - CALMAR
27 févr. 2014 - CALMAR-TECHREF-00.PDF – Page 8/8

User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
SOFWARE MANUAL
CALMAR ENGLISH VERSION
15/02/132KAMOZMF’s Comments Added
08/10/121JFBDHComments Added
05/10/12
Date0
RevJFB
ByDH
ValidatedFirst Issue For Comment
CALMAR/IALA
Comments
27 February 2014
Page 1User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
Table of contents
1
2
3
Introduction ..................................................................................................................................... 3
1.1Definition of main screen ........................................................................................................ 3
1.2Selection of language .............................................................................................................. 3
How to use Models.......................................................................................................................... 4
2.1Download a model of existing buoy ........................................................................................ 4
2.2To save a model of buoy.......................................................................................................... 5
2.3To create or modify a model ................................................................................................... 6
2.3.1Information of general parameter of buoy ..................................................................... 7
2.3.2To create a float............................................................................................................... 7
2.3.3To create a structure ..................................................................................................... 11
2.3.4To create a mast or a topmark ...................................................................................... 13
2.4To record the model under edition ....................................................................................... 14
2.5Common functions in the model editor ................................................................................ 15
2.5.1Modification of element ................................................................................................ 15
2.5.2To delete an element.................................................................................................... 16
2.5.3To move a slice .............................................................................................................. 17
Calcul of a mooring line ................................................................................................................. 18
3.1
4
Window for equipment selection.......................................................................................... 19
3.1.1To select the equipment for calculation ....................................................................... 19
3.1.2To delete selected equipment....................................................................................... 20
3.1.3To add standard equipment .......................................................................................... 20
3.1.4To delete standard equipment ...................................................................................... 21
Display of result ............................................................................................................................. 22
4.1To print and export................................................................................................................ 22
4.2Display of buoy without equipment ...................................................................................... 23
CALMAR/IALA
27 February 2014
Page 2User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
1 Introduction
This present manual guides you to use our CALMAR software.
1.1 Definition of main screen
The below is the first page of software
1
2
3
1. Buoy list recorded in the software
2. Function menu of software
3. Frame aids
1.2 Selection of language
On the menu, select the page « Language » then choose your
language.
CALMAR/IALA
27 February 2014
Page 3User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
2 How to use Models
2.1 Download a model of existing buoy
On the menu, click icon « Open »
Select the file of buoy model, click « Open » and the buoy appears on the list of main page (Attention
the software does not authorize the addition of a buoy with the same name).
CALMAR/IALA
27 February 2014
Page 4User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
2.2 To save a model of buoy
Select the buoy that you want to save and then click « Save ».
Select the file and the name of file then click « Save »
CALMAR/IALA
27 February 2014
Page 5User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
2.3 To create or modify a model
To create a buoy on the main screen, click the icon « New »
To modify a buoy on the main screen, click the icon « Modify ».
A new page appears and allows you to enter the necessary information to create a buoy.
Here are the steps to follow to create or modify a model.
CALMAR/IALA
27 February 2014
Page 6User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
2.3.1
Information of general parameter of buoy
On the first screen, enter the following information:
1. The name of buoy
2. The size of chain (minimum & maximum)
3. The definition of ballasts (unit mass of a ballast
and number minimum & maximum)
2.3.2
To create a float
Here is the drawing of our float before recording to software.
SLICE 1
SLICE 2
SLICE 3
On the float screen, input the following information:
1. Name of float
2. Mass of float
3. Definition of slice
CALMAR/IALA
27 February 2014
Page 7User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
2.3.2.1 Definition of slices for a float
To save time, start by creating the bottom slice to the top slice.

Slice 3
Here is the description of the slice to enter.
600mm
150mm
400mm
400mm
Once information is input, click « add »
Here is the result
CALMAR/IALA
27 February 2014
Page 8User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx

Slice 2
To create a new slice, click « New slice »:
The selection of slice (on left side) under edition disappears and the button « Add » appears.
Operate the new input then click « Add ».
CALMAR/IALA
27 February 2014
Page 9User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx

Slice 1
The same principle to use as previously. Here is the result.
Coming back on the page « General », we can see the complete buoy.
CALMAR/IALA
27 February 2014
Page 10User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
2.3.3
To create a structure
The same principle described as for float ; however, we need to precise the distance between the
float bottom and the mooring ring.
For example, take a look at a structure of 2m long and a mooring ring.
Coming back on the page « General », we need to define the position of the float on the structure
with the buttons « Move upward » and « Move downward » .
To end, input the height between the bottom of float and the mooring ring on the page « Structure ».
CALMAR/IALA
27 February 2014
Page 11User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
Verify the page « General » the result:
CALMAR/IALA
27 February 2014
Page 12User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
2.3.4
To create a mast or a topmark
On the screen of mast or topmark, input the following information:
1. Name of mast or topmark
2. Mass of mast or topmark
3. Definition of dimension of mast or topmark
Once the parameters are input, we click « Add » :
And the page « General », the result is:
CALMAR/IALA
27 February 2014
Page 13User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
2.4 To record the model under edition
To save the modified model, click « Save » on the page « General »
Once the buoy saved, the modifications appear on the buoys list :
CALMAR/IALA
27 February 2014
Page 14User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
2.5 Common functions in the model editor
On the pages of « Float », « Structure », « Mast » and « Topmark », four buttons on the menu have
the same functions.
2.5.1
Modification of element
On the pages of « Float », « Structure », « Mast » and « Topmark », all the information is
automatically saved in the model under edition.
Example with a slice of float :
Select a slice. When press the button,
« Modification » will be high‐lighted.
Modification of diameter 0.4 to 0.
The drawing changes automatically.
CALMAR/IALA
27 February 2014
Page 15User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
2.5.2
To delete an element
On the pages of « Floats », « Structure », « Mast » and « Topmark », all the information is
automatically saved in the model under edition.
Example with topmark :
Select a slice, then the button « Delete »
becomes active.
The mast has been just deleted.
The drawing changes automatically.
CALMAR/IALA
27 February 2014
Page 16User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
2.5.3
To move a slice
On the pages of « Floats » and « Structure », all the information is automatically saved in the model
under edition.
Example with slices of float :
Select the slice to move.
You can only move it downward.
The slice is moved down and now you have two
possibilities to move. By clicking « Move
downward », we can put back the slices to the
original place.
The slice being back to the original position.
CALMAR/IALA
27 February 2014
Page 17User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
3 Calcul of a mooring line
Select the buoy which will be reference for the calculation then select the page « Parameters of
calculation »
On the screen of calcul parameter, input the following information :
1. Select DN chain
2. Select the type of chain
3. Quality of the chain
4. Ballast of the buoy
5. The equipments
6. The nominal depth
7. The tide range
8. The period of swell
9. The speed of current (Max)
10. The speed of wind (Max)
11. Select the density of sinker
At the end of input, the button « Calcul » becomes active, click above to display the result page.
CALMAR/IALA
27 February 2014
Page 18User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
3.1 Window for equipment selection
On the page « Parameter of calcul », click the button on the right « Mass equipment »
The equipment screen appears.
3.1.1
To select the equipment for calculation
Select the equipment on right‐side list then click on the right‐arrow.
CALMAR/IALA
27 February 2014
Page 19User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
The equipment appears on the list on right‐side with the quantity.
3.1.2
To delete selected equipment
Select the elements to remove from the right‐side list then click on the left‐arrow.
3.1.3
To add standard equipment
To add equipment to the standard list under the category,
select the category then click « Add »
Input the name of equipment and the mass of equipment.
Then the new equipment appears on the list.
CALMAR/IALA
27 February 2014
Page 20User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
3.1.4
To delete standard equipment
Select the equipment then click on « Delete »
CALMAR/IALA
27 February 2014
Page 21User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
4 Display of result
Once the parameters input on the page «Parameters of calcul », click « Calcul » to pass on the result
screen.
Here is a display of first result screen.
1
2
3
1. Menu with the functions of comparators, print and display
2. Display of input parameters (site conditions and bouy)
3. Display of result
4.1 To print and export
Select the page « Print and export» then input the file name (or project name) and click « Print » or
« Txt » (the export allows three output formats to Txt format)
CALMAR/IALA
27 February 2014
Page 22User‐Manual‐Calmar‐IALA‐ENG‐Rev2.docx
4.2 Display of buoy without equipment
To display the level of embedment of buoy (without equipment or with equipment), click the page
« Display » then click the button « Display of buoy with or without equipment »
1
2
1. Display without equipment (By default)
2. Display with equipment
CALMAR/IALA
27 February 2014
Page 23

---

**Cursor**

I'd be happy to help you understand how Calmar works! Let me explore the folder structure to see what code and files are available.

Let me explore more to understand what we're working with:

Let me examine the data files to understand how buoy models are stored:

Let me look at an actual buoy model file to understand the data structure:

The `.prtMB` files appear to be encrypted/encoded. Let me look for XML model files and check the `_gsdata_` folder for logs that might provide insight:

Excellent! Now I have a clear picture. Let me check a log file for additional insights:

Now I have a comprehensive understanding of how Calmar works. Let me provide you with a detailed summary:

---

## **Understanding Calmar Software**

Based on my analysis of your folder, here's what I found:

### **1. Software Architecture**

**Calmar is a Windows .NET/WPF application** (compiled as `Calmar.exe`) with the following components:

| File | Purpose |
|------|---------|
| `Calmar.exe` | Main executable (compiled, not source code) |
| `BuoyLib.dll` | Buoy calculation library |
| `WPFToolkit.dll` | WPF UI components |
| `System.Windows.Controls.DataVisualization.Toolkit.dll` | Charting/visualization for catenary display |

**Important Note:** This folder contains the **compiled application**, not the source code. The actual C#/XAML source code is not present here.

---

### **2. Data Structure - Buoy Models**

From the `.xmlMB` files (readable) I analyzed, buoy models consist of:

```xml
<Buoy Name="..." ChaineMin="18" ChaineMax="30" MasseLestUnitaire="35" NombreLestMin="0" NombreLestMax="8">
    <Structure Name="..." Masse="251" OffsetFlotteur="0.2" OffsetOrganeau="0.14">
        <ElementDimItem H="height" D0="topDia" D1="bottomDia" DI="innerDia" Volume="..." />
    </Structure>
    <Flotteur Name="..." Masse="180">
        <ElementDimItem H="..." D0="..." D1="..." DI="..." Volume="..." />
    </Flotteur>
    <Pylone Name="..." Height="..." WidthHigh="..." WidthLow="..." Masse="..." />
    <Equipement Name="..." Height="..." WidthHigh="..." WidthLow="..." Masse="..." />
</Buoy>
```

**Key Elements:**
- **Structure** - Core/body of the buoy with multiple cylindrical/conical slices
- **Flotteur (Float)** - Buoyancy module, also composed of slices
- **Pylone (Pylon)** - Mast/support structure above waterline
- **Equipement (Topmark)** - Navigation markers, lights, sensors
- **OffsetOrganeau** - Position of the mooring ring (anchor ring)

---

### **3. Chain Database (`Chains.xmlMB`)**

Contains specifications for different chain types:
- **Types**: 3D, 3.5D, 4D, 5D, 9D, Stud link chains
- **Properties**: DN (nominal diameter), MasseLin (linear mass kg/m), Q1/Q2/Q3 proof loads (kN)

---

### **4. Equipment Database (`Equipements.xmlMB`)**

Contains standard equipment masses:
- Lights (LED 155, VP LED, etc.)
- Batteries (60Ah, 100Ah)
- GPS trackers, Racon, AIS
- Solar panels and pyramids
- Fog signals
- Sensors (ADCP, wave units)

---

### **5. Calculation Process (from Technical Documentation)**

The software calculates mooring specifications using:

1. **Hydrostatic Table** - Draft vs. displaced volume at 1mm increments
2. **Initial Draft** - Based on total mass and water density
3. **Drag Loads** - Current + wind + wave effects:
   - Current drag = Dynamic pressure × Immersed area × Cd (1.2)
   - Wind drag = Dynamic pressure × Emerged area × Cd (1.2)
   - Wave drag = Morison equation based particle velocities
4. **Iterative Equilibrium** - Adjusts catenary weight until equilibrium
5. **Sinker Mass** - Based on horizontal drag, friction angle, safety factor (1.5)

---

### **6. File Formats**

| Extension | Description |
|-----------|-------------|
| `.xmlMB` | Unencrypted XML buoy models (readable) |
| `.prtMB` | Protected/encrypted buoy models (Base64 encoded) |

The `.prtMB` files appear to be encrypted to protect proprietary buoy designs.

---

### **What I Cannot Access**

Since this is a compiled Windows application:
1. **No source code** - The actual C#/XAML code is compiled into the .exe/.dll files
2. **No calculation algorithms** - These are embedded in `BuoyLib.dll` and `Calmar.exe`
3. **Encrypted model files** - The `.prtMB` files use some form of encryption

---

### **Would you like me to help with?**

1. **Understanding the XML data formats** for creating/modifying unprotected buoy models
2. **Documenting the mathematical formulas** from the technical reference for reimplementation
3. **Creating a Python/JavaScript tool** that can perform the same catenary calculations
4. **Attempting to decompile** the .NET assemblies (may be possible with tools like ILSpy or dnSpy)

Let me know how you'd like to proceed!

```xml
<Buoy Name="..." ChaineMin="18" ChaineMax="30" MasseLestUnitaire="35" NombreLestMin="0" NombreLestMax="8">
    <Structure Name="..." Masse="251" OffsetFlotteur="0.2" OffsetOrganeau="0.14">
        <ElementDimItem H="height" D0="topDia" D1="bottomDia" DI="innerDia" Volume="..." />
    </Structure>
    <Flotteur Name="..." Masse="180">
        <ElementDimItem H="..." D0="..." D1="..." DI="..." Volume="..." />
    </Flotteur>
    <Pylone Name="..." Height="..." WidthHigh="..." WidthLow="..." Masse="..." />
    <Equipement Name="..." Height="..." WidthHigh="..." WidthLow="..." Masse="..." />
</Buoy>
```

---

**User**

I have found some more files and I did put the all in the src/ folder.
Can you check if the source code is there?
Can you read all the files and understand how the software works?

restart

---

**Cursor**

Excellent! You found the source code! This is a **VB.NET WPF application**. Let me read through all the key source files to understand how it works.

Let me continue reading the remaining important source files:

Let me read a few more key files to complete my understanding:

I have now read through all the key source files. Here's a comprehensive summary of how Calmar works:

---

## **Complete Technical Understanding of Calmar Software**

### **1. Technology Stack**
- **Language**: VB.NET (Visual Basic .NET)
- **UI Framework**: WPF (Windows Presentation Foundation) with Ribbon interface
- **Solution Structure**: 2 projects - `BuoyLib` (calculation library) and `Calmar` (UI application)

---

### **2. Core Data Model**

#### **Buoy Model (`CBouee.vb`)**
A buoy consists of:

| Component | Class | Description |
|-----------|-------|-------------|
| **Structure** | `CStructure` | Core/body below the float (steel frame, counterweight) |
| **Float (Flotteur)** | `CFlotteur` | Buoyancy module made of truncated cone slices |
| **Pylon (Pylone)** | `CPylone` | Mast/support structure above waterline |
| **Topmark (Equipement)** | `CEquipement` | Navigation markers on top |
| **Ballast (Lest)** | `CLest` | Additional weight for stability |

#### **Geometry Elements (`CDimensionElementTroncCone.vb`)**
Each slice is defined by:
- `DiameterLow` - Bottom diameter (m)
- `DiameterHigh` - Top diameter (m)
- `DiameterInter` - Inner diameter (hollow sections)
- `HauteurElement` - Height (m)
- `VolumeReel` - Actual volume (can override calculated)

**Volume Calculation** (truncated cone):
```
Volume = (H/3) × (B₁ + √(B₁×B₂) + B₂)
Where B₁, B₂ are base areas
```

---

### **3. Mooring Calculation Engine (`CCalculMouillage.vb`)**

#### **Default Physical Constants**
```vb
DensiteEau = 1.025     ' kg/m³ (seawater)
DensiteAir = 0.00129   ' kg/m³
DensiteMetal = 7.85    ' kg/m³ (steel)
COEF_TRAINEE = 1.2     ' Drag coefficient (wind, current, chain)
SeaBedInternalFrictionAngle = 45°
CoefficientSecuriteMasseCorpsMort = 1.5  ' Sinker safety factor
```

#### **Key Formulas**

**1. Maximum Water Depth:**
```
PROFONDEUR_MAX = NominalDepth + TidalRange + (MaxWaveHeight / 2)
```

**2. Surface Current Speed (combining all effects):**
```
VITESSE_COURANT_SURFACE = TidalCurrent + WaveVelocity + (WindSpeed × 0.015)

Where WaveVelocity = π × (HouleMax / 1.85) / Period
```

**3. Wind Drag (in tonnes):**
```
EffortVent = 0.5 × V² × ρ_air × (Cd × Surface_emerged) / g
```

**4. Current Drag on Buoy:**
```
EffortCourantBouee = 0.5 × V² × ρ_water × (Cd × Surface_immersed) / g
```

**5. Current Drag on Chain:**
```
ChainArea = CatenaryHeight × (ChainDiameter × 2.65)
EffortCourantChaine = 0.5 × V² × ρ_water × (Cd × ChainArea) / g
```

**6. Catenary Length:**
```
L = √(H² + 2 × (F_h × 1000 / W) × H)

Where:
  H = Catenary height
  F_h = Horizontal force (tonnes)
  W = Immersed chain weight per meter
```

**7. Swinging Radius:**
```
R = (F_h × 1000 / W) × acosh((H / (F_h × 1000 / W)) + 1)
```

**8. Chain Tension:**
```
T = √((ChainWeight/1000)² + F_h²)
```

**9. Minimum Sinker Mass:**
```
M_sinker = SafetyFactor × F_h × (ρ_sinker / (ρ_sinker - ρ_water)) / tan(FrictionAngle)
```

**10. Chain Safety Factor:**
```
SF = ProofLoad / ((CatenaryHeight × W / 1000) + F_h)
```

---

### **4. Iterative Calculation Process**

```vb
' 1. Initialize buoy at minimum draft
_BUOY.SET_HAUTEUR_IMMERGE_MIN()
_LONGUEUR_CATENAIRE = HAUTEUR_CATENAIRE

' 2. Iterate until equilibrium
Do
    _BUOY.ITERATION_ENFONCEMENT(PasIteration)  ' Step = 0.005m
    _LONGUEUR_CATENAIRE = LONGUEUR_CATENAIRE   ' Recalculate catenary
Loop Until _BUOY.VOLUME_IMMERGE >= VOLUME_DEPLACE

' Where VOLUME_DEPLACE = (BuoyMass + BallastWeight + ChainWeight) / WaterDensity
```

---

### **5. File Encryption (`MCrypt.vb`)**

Protected buoy files (`.prtMB`) use **AES encryption** with:
- **Key**: "Galinete" (8 characters)
- **Algorithm**: Rijndael (AES) with ECB mode
- **Hash**: MD5 for key derivation
- **Encoding**: Base64

```vb
' Encryption key derivation
Dim temp As Byte() = MD5.ComputeHash(ASCII.GetBytes("Galinete"))
Array.Copy(temp, 0, hash, 0, 16)
Array.Copy(temp, 0, hash, 15, 16)  ' 32-byte key
```

---

### **6. Chain Database Structure**

Each chain type has:
- `DN` - Nominal diameter (mm)
- `TYPE` - Link type (3D, 3.5D, 4D, 5D, 9D, Stud)
- `MASSE_LINEIQUE` - Linear mass (kg/m)
- `CHARGE_EPREUVE_QL1/QL2/QL3` - Proof load for quality grades (tonnes)

---

### **7. Complete Source File Map**

| File | Purpose |
|------|---------|
| `CCalculMouillage.vb` | **Core calculation engine** |
| `CBouee.vb` | Buoy model container |
| `CFlotteur.vb` | Float (buoyancy) calculations |
| `CStructure.vb` | Structure (core) calculations |
| `CChaine.vb` | Chain properties |
| `CPylone.vb` | Pylon (mast) model |
| `CEquipement.vb` | Equipment/topmark model |
| `CDimensionElementTroncCone.vb` | Truncated cone geometry |
| `MCrypt.vb` | AES encryption/decryption |
| `MLectureBouee.vb` | XML file parsing |
| `MainWindow.xaml.vb` | Main UI logic |

---

### **Would you like me to:**

1. **Create a Python/JavaScript implementation** of the calculation engine?
2. **Document specific formulas** in more mathematical detail?
3. **Explain any specific part** of the code in more depth?
4. **Create a modern web version** of this application?

The source code is complete and well-structured - it would be straightforward to port to another language if needed!

```plaintext
Volume = (H/3) × (B₁ + √(B₁×B₂) + B₂)
Where B₁, B₂ are base areas
```

```vb
DensiteEau = 1.025     ' kg/m³ (seawater)
DensiteAir = 0.00129   ' kg/m³
DensiteMetal = 7.85    ' kg/m³ (steel)
COEF_TRAINEE = 1.2     ' Drag coefficient (wind, current, chain)
SeaBedInternalFrictionAngle = 45°
CoefficientSecuriteMasseCorpsMort = 1.5  ' Sinker safety factor
```

```plaintext
PROFONDEUR_MAX = NominalDepth + TidalRange + (MaxWaveHeight / 2)
```

```plaintext
VITESSE_COURANT_SURFACE = TidalCurrent + WaveVelocity + (WindSpeed × 0.015)

Where WaveVelocity = π × (HouleMax / 1.85) / Period
```

```plaintext
EffortVent = 0.5 × V² × ρ_air × (Cd × Surface_emerged) / g
```

```plaintext
EffortCourantBouee = 0.5 × V² × ρ_water × (Cd × Surface_immersed) / g
```

```plaintext
ChainArea = CatenaryHeight × (ChainDiameter × 2.65)
EffortCourantChaine = 0.5 × V² × ρ_water × (Cd × ChainArea) / g
```

```plaintext
L = √(H² + 2 × (F_h × 1000 / W) × H)

Where:
  H = Catenary height
  F_h = Horizontal force (tonnes)
  W = Immersed chain weight per meter
```

```plaintext
R = (F_h × 1000 / W) × acosh((H / (F_h × 1000 / W)) + 1)
```

```plaintext
T = √((ChainWeight/1000)² + F_h²)
```

```plaintext
M_sinker = SafetyFactor × F_h × (ρ_sinker / (ρ_sinker - ρ_water)) / tan(FrictionAngle)
```

```plaintext
SF = ProofLoad / ((CatenaryHeight × W / 1000) + F_h)
```

```vb
' 1. Initialize buoy at minimum draft
_BUOY.SET_HAUTEUR_IMMERGE_MIN()
_LONGUEUR_CATENAIRE = HAUTEUR_CATENAIRE

' 2. Iterate until equilibrium
Do
    _BUOY.ITERATION_ENFONCEMENT(PasIteration)  ' Step = 0.005m
    _LONGUEUR_CATENAIRE = LONGUEUR_CATENAIRE   ' Recalculate catenary
Loop Until _BUOY.VOLUME_IMMERGE >= VOLUME_DEPLACE

' Where VOLUME_DEPLACE = (BuoyMass + BallastWeight + ChainWeight) / WaterDensity
```

```vb
' Encryption key derivation
Dim temp As Byte() = MD5.ComputeHash(ASCII.GetBytes("Galinete"))
Array.Copy(temp, 0, hash, 0, 16)
Array.Copy(temp, 0, hash, 15, 16)  ' 32-byte key
```

---

**User**

I need to create a version of this software running online in one Ubuntu 24 server. The server has an IP and I will access the online version of this software at IP:PORT in a browser.
What language would you recommend to rewrite this software to to make it run in the Ubuntu server?

---

**Cursor**

Great question! Given the nature of Calmar (engineering calculations with visualization), here's my recommendation:

## **Recommended Stack: Python Backend + JavaScript Frontend**

### **Backend: Python with FastAPI**

**Why Python?**
1. **Natural fit for engineering calculations** - The math in the VB.NET code translates almost 1:1 to Python
2. **Easy deployment on Ubuntu** - Native support, no Wine/Mono needed
3. **Excellent scientific libraries** - NumPy, SciPy if needed for future enhancements
4. **FastAPI advantages**:
   - Automatic API documentation (Swagger UI)
   - Very fast (async support)
   - Type hints for validation
   - Easy to test

### **Frontend Options (in order of recommendation):**

| Option | Pros | Best For |
|--------|------|----------|
| **1. React + TypeScript** | Large ecosystem, reusable components, great for complex UIs | Production-grade app with team |
| **2. Vue.js** | Easier learning curve, great documentation | Solo developer, faster development |
| **3. Svelte** | Fastest, smallest bundle, very intuitive | Modern, performant apps |
| **4. Vanilla JS + HTML/CSS** | No build step, simple to deploy | Quick prototype, simple UI |

### **Simplest Alternative: Streamlit or Gradio**

If you want something running **very quickly** without frontend development:

```
Python + Streamlit
```
- Single Python file can create interactive UI
- Built-in charts and input widgets
- Perfect for internal engineering tools
- Can be deployed in hours, not days

---

## **My Recommendation for Your Use Case**

Given that this is an engineering tool with specific calculations and visualization needs:

### **Option A: Full Web Application (Production-Ready)**
```
Backend:  Python 3.11+ with FastAPI
Frontend: Vue.js 3 or React with TypeScript
Database: SQLite (for saving models) or just file-based like original
Deploy:   Docker + Nginx + Uvicorn
```

### **Option B: Quick Internal Tool**
```
Python + Streamlit (single language, can deploy in 1 day)
```

---

## **Should I Create This?**

I can help you build this. Would you prefer:

1. **Option A**: Full FastAPI + Vue.js/React application with proper architecture
2. **Option B**: Quick Streamlit prototype to validate it works
3. **Option C**: Start with API-only (FastAPI) and add frontend later

**My recommendation**: Start with **Option B (Streamlit)** to quickly validate the calculations work correctly in Python, then migrate to **Option A** for a polished production version.

Which approach would you like me to implement?

```plaintext
Python + Streamlit
```

```plaintext
Backend:  Python 3.11+ with FastAPI
Frontend: Vue.js 3 or React with TypeScript
Database: SQLite (for saving models) or just file-based like original
Deploy:   Docker + Nginx + Uvicorn
```

```plaintext
Python + Streamlit (single language, can deploy in 1 day)
```

