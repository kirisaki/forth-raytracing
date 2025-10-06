\ Structure of a sphere
begin-structure sphere%
  vec3% +field s-center
  ffield: s-radius
end-structure

\ Initialize sphere pool
: sphere-pool-create ( arena -- pool )
  sphere% 8 pool-init
;

\ Create a new sphere
: sphere-new ( center  pool -- sphere-addr ) ( radius -- )
  locals| sp c |
  sp pool-alloc
  dup s-center c swap vec3-move
  dup s-radius f!
;

\ Display sphere
: .sphere ( sphere-addr -- )
  s" Sphere:" type cr
  s" Center: " type
  dup s-center .v cr
  s" Radius: " type
  s-radius f@ f. cr
;

\ Structure for hit record
begin-structure hit-record%
  vec3% +field h-point
  vec3%   +field h-normal
  ffield: h-t-val
  field: h-front-face
end-structure

\ Create a new empty hit record
: hit-record-empty ( pool -- hit-record-addr )
  pool-alloc
  dup h-point 0e 0e 0e v!
  dup h-normal 0e 0e 0e v!
  dup h-t-val 0e f!
  dup h-front-face swap 0 !
;

\ Display hit record
: .hit-record ( hr-addr -- )
  s" Hit Record:" type cr
  s" Point: " type
  dup h-point .v cr
  s" Normal: " type
  dup h-normal .v cr
  s" t-value: " type
  dup h-t-val f@ f. cr
  s" Front face: " type
  dup h-front-face @ if ." true" else ." false" then cr
;

\ Set face normal
: hit-record-set-face-normal ( ray outward rec -- )
  locals| rec outward ray |
  ray r-direction outward vdot f0< dup if
    rec h-front-face !
    outward rec h-normal vec3-move
  else
    rec h-front-face !
    outward v@ fnegate frot fnegate frot fnegate frot rec h-normal v!
  then
;

\ Detect ray-sphere intersection
: hit-sphere ( center ray vp -- ) ( f-radius -- f )
  locals| vp ray center |
  vp vec3-zero locals| oc |
  ray r-origin center oc v- 
  vp vec3-zero locals| dir |
  ray r-direction dir vec3-move 
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
  oc vp pool-free
  dir vp pool-free
;

\ Tests
: test-sphere ( -- )
  cr ." ---test-sphere" cr
  1024 arena-create locals| arena |
  arena vec3-pool-create locals| vp |
  arena ray-pool-create locals| rp |
  arena sphere-pool-create locals| sp |
  
  \ Create center and radius
  0e 0e -1e vp vec3-new locals| center |

  \ Create sphere
  center 0.5e sp sphere-new locals| sphere |

  \ Print sphere
  sphere .sphere

  \ Create origin and direction vectors
  0e 0e 0e vp vec3-new locals| o |
  0e 0e -1e vp vec3-new locals| d |

  \ Create ray
  o d rp ray-new locals| r |

  \ Print ray
  r .ray

  \ Test hit-sphere
  r center vp 0.5e hit-sphere fdup f0> if
    s" Ray hits sphere at t=" type f. cr
  else
    s" Ray misses sphere." type cr
  then

  \ Free resources
  sphere sp pool-free
  center vp pool-free
  o vp pool-free
  d vp pool-free
  r rp pool-free

  cr
;