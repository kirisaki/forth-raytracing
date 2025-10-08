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
: vrand ( u v-addr -- u )
  swap frand frand frand 
  swap dup vz f! dup vy f! vx f!
;

\ Generate a random vector in [min, max)]
: vrand-range ( u v-addr -- u ) ( min max -- )
  swap fover fover 
  frand-range
  2 fpick 2 fpick  
  frand-range
  3 fpick 3 fpick
  frand-range
  swap dup vz f! dup vy f! vx f!
  fdrop fdrop
;

\ Generate a random vector in unit sphere
: vrand-in-unit-sphere ( u v-addr -- u )
  swap >r
  begin
    -1e 1e dup r> swap vrand-range >r
    dup vlength2 1e f< if
      drop r>
      exit
    then
  again
;

\ Generate a random vector in hemisphere
: vrand-in-hemisphere ( rng normal-addr out vp -- rng )
  locals| vp out n gen |
  vp pool-alloc locals| r |
  gen r vrand-in-unit-sphere to gen
  r n vdot 0e f< if
    -1e r out vmul
  else
    r out vec3-move
  then
  gen
  r vp pool-free
;

\ \ Fenerate a random vector in unit disk
\ : vrand-in-unit-disk ( u v-addr -- u )
\   begin
\     -1e 1e frand-range
\     -1e 1e frand-range
\     0e vec3-new
\     dup vlength2 1e f< if
\       exit
\     then
\     drop
\   again
\ ;

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

  
  1024 arena-create locals| arena |
  arena vec3-pool-create locals| vp |
  vp pool-alloc locals| v |
  ." 5 random vectors in [0.0, 1.0): " cr
  1234567890
  5 0 do
    v vrand v .v
  loop
  drop
  cr cr

  ." 5 random vectors in [-1.0, 1.0): " cr
  1234567890
  5 0 do
    -1.0e 1.0e v vrand-range v .v
  loop
  drop
  cr cr

  ." 5 random vectors in unit sphere: " cr
  1234567890
  5 0 do
    v vrand-in-unit-sphere v .v
  loop
  drop
  cr cr

  ." 5 random vectors in hemisphere (normal = 0,1,0): " cr
  1234567890
  0e 1e 0e vp vec3-new locals| n |
  5 0 do
    n v vp vrand-in-hemisphere v .v
  loop
  drop

  arena arena-destroy
  cr
;