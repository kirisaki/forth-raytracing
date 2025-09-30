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

  cr
;