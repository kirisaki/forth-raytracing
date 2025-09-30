begin-structure ray%
  vec3% +field r-origin
  vec3% +field r-direction
end-structure

\ Initialize ray pool
: ray-pool-create ( arena -- pool )
  ray% 8 pool-init
;

\ Create a new ray
: ray-new ( origin direction pool -- ray-addr )
  locals| p d o |
  p pool-alloc
  dup r-origin o swap vec3-move
  dup r-direction d swap vec3-move
;

\ Display ray
: .ray ( ray-addr -- )
  s" Ray:" type cr
  s" Origin: " type
  dup r-origin .v cr
  s" Direction: " type
  r-direction .v cr
;

\ Get point along ray at parameter t
: ray-at ( ray-addr out-addr pool -- ) ( t -- )
  locals| p out r |
  p vec3-zero 
  r r-direction over vmul
  dup r r-origin out v+
  p pool-free
;

\ Convert ray into color vector
: ray-color ( ray-addr out-addr pool -- )
  locals| vp col ray |
  vp vec3-zero locals| unit-dir |
  ray r-direction unit-dir vunit
  unit-dir vy f@ 1.0e f+ 2.0e f/
  fdup 1.0e fswap f- 

  1e 1e 1e vp vec3-new locals| white |
  white vmul=
  0.5e 0.7e 1.0e vp vec3-new locals| blue |
  blue vmul=
  white blue v+=
  white col vec3-move

  blue vp pool-free
  white vp pool-free
  unit-dir vp pool-free
;

\ Color to a pixel
: pixel-color ( color -- u u u )
  locals| color |
  color vx f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
  color vy f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
  color vz f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
;


\ Tests
: test-ray ( -- )
  cr ." ---test-ray" cr
  1024 arena-create locals| arena |
  arena vec3-pool-create locals| vp |
  arena ray-pool-create locals| rp |
  
  \ Create origin and direction vectors
  1e 2e 3e vp vec3-new locals| o |
  4e 5e 6e vp vec3-new locals| d |
  
  \ Create ray
  o d rp ray-new locals| r |
  
  \ Print ray
  r .ray

  \ Get point at t=2.0
  vp vec3-zero locals| at |
  2.0e r at vp ray-at
  s" Point at t=2.0: " type
  at .v cr

  \ Get ray color
  vp vec3-zero locals| col |
  r col vp ray-color
  s" Ray color: " type
  col .v cr

  s" Pixel color(B, G, R): " type
  col pixel-color . . .  \ Print RGB values
  cr

  cr
;