: rand ( u -- u )
  \ Xorshift64 PRNG
  dup 0= if drop 1 then
  dup 12 lshift xor
  dup 25 rshift xor
  dup 27 lshift xor
  dup 2685821657736338717 *
;

: frand ( u -- u ) ( -- f )
  dup rand 11 rshift
  s>f 9007199254740992e f/ 
;

: test-random ( -- )
  cr ." ---test-random" cr
  ." 10 random numbers: " cr
  1234567890
  10 0 do
    rand dup . 
  loop
  cr
  ." 10 floating point random numbers: " cr
  1234567890
  10 0 do
    frand f. 
  loop
;