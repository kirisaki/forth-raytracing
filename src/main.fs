include ./pnm.fs
include ./vector.fs
include ./ray.fs

: hit-sphere ( center ray -- ) ( f-radius -- f )
  locals| ray center |
  ray origin center v- locals| oc |
  ray direction locals| dir |
  dir vlength2 \ r a
  oc dir vdot \ r a b/2
  oc vlength2 3 fpick fdup f* f- \ r a b/2 c
  1 fpick fdup f* \ r a b/2 c (b/2)^2 
  3 fpick 2 fpick f* \ r a b/2 c b^2 ac
  f- \ r a b/2 c d
  fdup f0< if
    5 0 do fdrop loop -1e \ no hit
  else
    fsqrt
    2 fpick fnegate fswap f- 3 fpick f/
    4 0 do fdrop loop
  then
;

: ray-color ( ray -- color )
  locals| ray |
  0e 0e -1e vec3-new ray 0.5e hit-sphere fdup f0< if
    fdrop
    ray direction vunit
    vy f@ 1e f+ 2e f/
    fdup 1e fswap f-
    1e 1e 1e vec3-new
    vmul
    0.5e 0.7e 1.0e vec3-new
    vmul
    v+
  else
    ray at 0e 0e -1e vec3-new v- vunit
    1e 1e 1e vec3-new v+
    2e vdiv
  then
;

: pixel-color ( color -- u u u )
  locals| color |
  color vx f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
  color vy f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
  color vz f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
;

: generate-pnm ( u-width u-height -- width-u height-u c-addr )
  locals| h w |
  w h * 3 * allocate throw locals| addr |
  3.555555e 0e 0e vec3-new locals| horizontal | \ 3.555... = viewport height(2.0) * aspect ratio(16/9)
  0e 2e 0e vec3-new locals| vertical |
  0e 0e 0e vec3-new locals| orig |
  horizontal 2e vdiv locals| llc-h |
  vertical 2e vdiv locals| llc-v |
  0e 0e 1e vec3-new locals| llc-f |
  orig llc-h v- llc-v v- llc-f v- locals| lower-left-corner |
  h 1- 0 swap do
    w 0 do
      orig
      lower-left-corner
      horizontal i s>f w 1- s>f f/ vmul v+
      vertical j s>f h 1- s>f f/ vmul v+
      orig v-
      ray-new ray-color pixel-color
      h 1- j - w * i + 3 * addr +
      dup 2 +
      rot swap c!
      dup 1 +
      rot swap c!
      c!
    loop
  -1 +loop
  w h
  addr
;

384 216 generate-pnm
\ 4 3 generate-pnm
s" test1.pnm" write-pnm

\ test-vector
\ test-ray

\ 1e 2e 3e vec3-alloc orig
\ 2e 3e 4e vec3-alloc dir
\ orig dir ray-alloc r
\ create x vec3% allot
\ r ray-color
\ dup .v cr
\ pixel-color . . . cr

." done." cr
bye