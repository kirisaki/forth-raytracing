\ Generate a random number
: rand ( u -- u )
  \ Xorshift64 PRNG
  dup 0= if drop 1 then
  dup 12 rshift xor
  dup 25 lshift xor
  dup 27 rshift xor
  2685821657736338717 *
;

\ Generate a floating point random number in [0.0, 1.0)
: frand ( u -- u ) ( -- f )
  rand dup 11 rshift
  s>f 9007199254740992e f/ 
;

\ Generate a floating point random number in [min, max)
: frand-range ( u -- u ) ( min max -- )
  fover fover fswap f-
  frand f*
  2 fpick f+
  fnip fnip
;

\ Generate a random vector
: vrand ( u -- u v-addr )
  frand frand frand vec3-new
;

\ Generate a random vector in [min, max)]
: vrand-range ( u -- u v-addr ) ( min max -- )
  fover fover 
  frand-range
  2 fpick 2 fpick  
  frand-range
  3 fpick 3 fpick
  frand-range vec3-new
  fdrop fdrop
;

\ Generate a random vector in unit sphere
: vrand-in-unit-sphere ( u -- u v-addr )
  begin
    -1e 1e vrand-range
    dup vlength2 1e f< if
      exit
    then
    drop
  again
;

\ Generate a random vector in hemisphere
: vrand-in-hemisphere ( normal-addr u -- u v-addr )
  swap
  locals| normal |
  vrand-in-unit-sphere
  dup normal vdot 0e f< if
    -1e vmul
  then
;

\ Fenerate a random vector in unit disk
: vrand-in-unit-disk ( u -- u v-addr )
  begin
    -1e 1e frand-range
    -1e 1e frand-range
    0e vec3-new
    dup vlength2 1e f< if
      exit
    then
    drop
  again
;

: test-random ( -- )
  cr ." ---test-random" cr
  ." 10 random numbers: " cr
  1234567890
  10 0 do
    rand dup . 
  loop
  drop
  cr cr

  ." 10 floating point random numbers: " cr
  1234567890
  10 0 do
    frand f. 
  loop
  drop
  cr cr

  ." 10 floating point random numbers in [5.0, 10.0): " cr
  1234567890
  10 0 do
    5.0e 10.0e frand-range f.
  loop
  drop
  cr cr

  ." 5 random vectors in [0.0, 1.0): " cr
  1234567890
  5 0 do
    vrand .v
  loop
  drop
  cr cr

  ." 5 random vectors in [-1.0, 1.0): " cr
  1234567890
  5 0 do
    -1.0e 1.0e vrand-range .v
  loop
  drop
  cr cr

  ." 5 random vectors in unit sphere: " cr
  1234567890
  5 0 do
    vrand-in-unit-sphere .v
  loop
  drop
  cr

  ." 5 random vectors in hemisphere (normal = (0,1,0)): " cr
  1234567890
  5 0 do
    0e 1e 0e vec3-new swap vrand-in-hemisphere .v
  loop
  drop
  cr

  ." 5 random vectors in unit disk: " cr
  1234567890
  5 0 do
    vrand-in-unit-disk .v
  loop
  drop
  cr
;