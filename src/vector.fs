begin-structure vec3%
  ffield: vx
  ffield: vy
  ffield: vz
end-structure

' vec3% alias point3%
' vec3% alias color%

\ Initialize vector pool
: vec3-pool-create ( arena -- pool )
  vec3% 8 pool-init
;

\ New vector
: vec3-new ( fx fy fz pool -- addr )
  pool-alloc >r
  r@ vz f!  r@ vy f!  r@ vx f!
  r>
;

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
: v+ ( v1-addr v2-addr out-addr -- v3-addr )
  -rot
  locals| v2 v1 |

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
  vz f!
;

\ subtract vectors
: v- ( v1-addr v2-addr out-addr -- )
  -rot
  locals| v2 v1 |

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
  vz f!
;

\ Multipuly vector by vector (Hadamard product)
: vhprod ( v1-addr v2-addr out-addr -- )
  -rot
  locals| v2 v1 |
  v1 vx f@ v2 vx f@ f* dup vx f!
  v1 vy f@ v2 vy f@ f* dup vy f!
  v1 vz f@ v2 vz f@ f* vz f!
;

\ multiply vector by scalar
: vmul ( v1-addr out-addr -- ) ( f -- )
  swap
  locals| v |

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
  vz f!
;

\ divide vector by scalar
: vdiv ( v1-addr out-addr -- ) ( f -- )
  1e fswap f/
  vmul
;
  
\ dot product
: vdot ( v1-addr v2-addr -- ) ( -- f )
  locals| v2 v1 |
  v1 vx f@ v2 vx f@ f*
  v1 vy f@ v2 vy f@ f* f+
  v1 vz f@ v2 vz f@ f* f+
;

\ cross product
: vcross ( v1-addr v2-addr out-addr -- )
  -rot
  locals| v2 v1 |

  v1 vy f@ v2 vz f@ f*
  v1 vz f@ v2 vy f@ f* f-
  v1 vz f@ v2 vx f@ f*
  v1 vx f@ v2 vz f@ f* f-
  v1 vx f@ v2 vy f@ f*
  v1 vy f@ v2 vx f@ f* f-
  dup vz f! dup vy f! vx f!
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
: vunit ( v1-addr v2-addr -- )
  over vlength fdup 0e f= if
    fdrop 1e
  then
  vdiv
;

: test-vector ( -- )
  1024 arena-create locals| arena |
  arena vec3-pool-create locals| vp |

  1e 2e 3e vp vec3-new 
  1e 1e 5e vp vec3-new
  vp pool-alloc
  locals| c b a |

  s" ---vector test" type cr
  s" a b" type cr
  a .v cr
  b .v cr
  
  s" v+" type cr
  a b c v+
  c .v cr

  s" v-" type cr
  a b c v-
  c .v cr

  s" vmul" type cr
  a c 2e vmul
  c .v cr

  s" vdiv" type cr
  a c 2e vdiv
  c .v cr

  s" vdot" type cr
  a b vdot f. cr


  s" vcross" type cr
  a b c vcross
  c .v cr

  s" vhadamard" type cr
  a b c vhprod
  c .v cr

  s" vlength" type cr
  a vlength f. cr

  s" vlength2" type cr
  a vlength2 f. cr

  s" vunit" type cr
  a c vunit
  c .v cr
  cr

  vp pool-free
  arena arena-destroy
;