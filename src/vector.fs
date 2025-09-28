begin-structure vec3%
  ffield: vx
  ffield: vy
  ffield: vz
end-structure

' vec3% alias point3%
' vec3% alias color%

\ New empty vector
: vec3-empty ( -- addr ) vec3% allocate throw ;

\ New vector
: vec3-new ( fx fy fz -- addr )
  vec3-empty >r
  r@ vz f!  r@ vy f!  r@ vx f!
  r> ;

' vec3-new alias color-new

: vec3-move ( src dst -- ) vec3% move ;

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
: v+ ( v1-addr v2-addr -- v3-addr )
  locals| v2 v1 |
  vec3-empty

  v1 vx f@
  v2 vx f@
  f+
  dup vx f!

  v1 vy f@
  v2 vy f@
  f+
  dup vy f!

  v1 vz f@
  v2 vz f@
  f+
  dup vz f!
;

\ subtract vectors
: v- ( v1-addr v2-addr -- v3-addr )
  locals| v2 v1 |
  vec3-empty

  v1 vx f@
  v2 vx f@
  f-
  dup vx f!

  v1 vy f@
  v2 vy f@
  f-
  dup vy f!

  v1 vz f@
  v2 vz f@
  f-
  dup vz f!
;

\ Multipuly vector by vector (Hadamard product)
: vhprod ( v1-addr v2-addr -- v3-addr )
  locals| v2 v1 |
  vec3-empty
  v1 vx f@ v2 vx f@ f* dup vx f!
  v1 vy f@ v2 vy f@ f* dup vy f!
  v1 vz f@ v2 vz f@ f* dup vz f!
;

\ multiply vector by scalar
: vmul ( v1-addr -- v2-addr ) ( f -- )
  locals| v |
  vec3-empty

  fdup
  v vx f@
  f*
  dup vx f!

  fdup
  v vy f@
  f*
  dup vy f!

  v vz f@
  f*
  dup vz f!
;

\ divide vector by scalar
: vdiv ( v1-addr -- v2-addr ) ( f -- )
  1e fswap f/
  vmul
;
  
\ dot product
: vdot ( v1-addr v2-addr -- f )
  locals| v2 v1 |
  v1 vx f@ v2 vx f@ f*
  v1 vy f@ v2 vy f@ f* f+
  v1 vz f@ v2 vz f@ f* f+
;

\ cross product
: vcross ( v1-addr v2-addr -- v3-addr )
  locals| v2 v1 |
  vec3-empty

  v1 vy f@ v2 vz f@ f*
  v1 vz f@ v2 vy f@ f* f-
  v1 vz f@ v2 vx f@ f*
  v1 vx f@ v2 vz f@ f* f-
  v1 vx f@ v2 vy f@ f*
  v1 vy f@ v2 vx f@ f* f-
  dup vz f! dup vy f! dup vx f!
;

\ length
: vlength ( v-addr -- ) ( -- f )
  locals| v |
  v vx f@ fdup f*
  v vy f@ fdup f* f+
  v vz f@ fdup f* f+ fsqrt
;

\ length squared
: vlength2 ( v-addr -- ) ( -- f )
  locals| v |
  v vx f@ fdup f*
  v vy f@ fdup f* f+
  v vz f@ fdup f* f+
;

\ unit vector
: vunit ( v1-addr -- v2-addr )
  dup vlength fdup 0e f= if
    fdrop 1e
  then
    vdiv
;

\ Reflect vector v about normal n
: vreflect ( uv n -- v )
  locals| n v |
  v n vdot 2e f* n vmul dup v swap v- swap free throw
;

\ Refract vector uv with normal n and refraction ratio etai_over_etat
: vrefract ( uv n -- refracted ) ( etai_over_etat -- )
  locals| n uv |
  uv n vdot fnegate 1e fmin
  n vmul dup uv v+ swap free throw dup vmul swap free throw
  dup vlength2 1e fswap f- fabs fsqrt fnegate n vmul over v+ swap free throw
;

: test-vector ( -- )
  1e 2e 3e   vec3-new 
  10e 20e 30e vec3-new
  vec3% allocate throw
  locals| c b a |

  s" ---vector test" type cr
  s" a b" type cr
  a .v cr
  b .v cr
  
  s" v+" type cr
  a b v+
  .v cr

  s" v-" type cr
  a b v-
  .v cr

  s" vmul" type cr
  a 2e vmul
  .v cr

  s" vdiv" type cr
  a 2e vdiv
  .v cr

  s" vdot" type cr
  a b vdot f. cr

  s" vcross" type cr
  a b vcross
  .v cr

  s" vlength" type cr
  a vlength f. cr

  s" vlength2" type cr
  a vlength2 f. cr

  s" vunit" type cr
  a vunit
  .v cr
  cr
;