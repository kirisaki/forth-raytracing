begin-structure vec3%
  ffield: vx
  ffield: vy
  ffield: vz
end-structure

vec3% constant point3%
vec3% constant color%

\ Allocate a vec3 and initialize it
: vec3-alloc  ( fx fy fz "<name>" -- )
  create
    vec3% allocate throw  dup >r
    r@ vz f!   r@ vy f!   r@ vx f!
    r> ,
  does>  ( -- addr )
    @ ;

\ Free a vec3
: vec3-free! ( "<name>" -- )
  ' dup >body @
  free throw
  0 swap >body ! ;

\ print vector
: .v ( v -- )
  ." ("
  dup vx f@ f.
  ." , "
  dup vy f@ f.
  ." , "
  vz f@ f.
  ." )"
;

\ add vectors
: v+ ( v1 v2 v3 -- )
  locals| v3 v2 v1 |

  v1 vx f@
  v2 vx f@
  f+
  v3 vx f!

  v1 vy f@
  v2 vy f@
  f+
  v3 vy f!

  v1 vz f@
  v2 vz f@
  f+
  v3 vz f!
;

\ subtract vectors
: v- ( v1 v2 v3 -- )
  locals| v3 v2 v1 |
  v1 vx f@
  v2 vx f@
  f-
  v3 vx f!

  v1 vy f@
  v2 vy f@
  f-
  v3 vy f!

  v1 vz f@
  v2 vz f@
  f-
  v3 vz f!
;

\ multiply vector by scalar
: vmul ( v s v2 -- )
  locals| v2 v |

  fdup
  v vx f@
  f*
  v2 vx f!

  fdup
  v vy f@
  f*
  v2 vy f!

  v vz f@
  f*
  v2 vz f!
;

\ divide vector by scalar
: vdiv ( v s v2 -- )
  1e fswap f/
  vmul
;
  
\ dot product
: vdot ( v1 v2 -- f )
  locals| v2 v1 |
  v1 vx f@ v2 vx f@ f*
  v1 vy f@ v2 vy f@ f* f+
  v1 vz f@ v2 vz f@ f* f+
;

\ cross product
: vcross ( v1 v2 v3 -- )
  locals| v3 v2 v1 |
  v1 vy f@ v2 vz f@ f*
  v1 vz f@ v2 vy f@ f* f-
  v1 vz f@ v2 vx f@ f*
  v1 vx f@ v2 vz f@ f* f-
  v1 vx f@ v2 vy f@ f*
  v1 vy f@ v2 vx f@ f* f-
  v3 vz f! v3 vx f! v3 vy f!
;

\ length
: vlength ( v -- f )
  locals| v |
  v vx f@ fdup f*
  v vy f@ fdup f* f+
  v vz f@ fdup f* f+ fsqrt
;

\ length squared
: vlength2 ( v -- f )
  locals| v |
  v vx f@ fdup f*
  v vy f@ fdup f* f+
  v vz f@ fdup f* f+
;

\ unit vector
: vunit ( v v2 -- )
  locals| v2 v |
  v vlength fdup 0e f= if
    drop 1e
  then
    v v2 vdiv
;