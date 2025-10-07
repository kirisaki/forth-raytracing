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

\ Initialize hit record pool
: hit-record-pool-create ( arena -- pool )
  hit-record% 8 pool-init
;

\ Create a new empty hit record
: hit-record-empty ( hrp -- hit-record-addr )
  pool-alloc
  dup h-point 0e 0e 0e v!
  dup h-normal 0e 0e 0e v!
  dup h-t-val 0e f!
  dup h-front-face 0 swap !
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
  h-front-face @ if ." true" else ." false" then cr
;

\ Copy hit record
: hit-record-move ( src dst -- )
  over h-point over h-point vec3-move
  over h-normal over h-normal vec3-move
  over h-t-val f@ dup h-t-val f!
  swap h-front-face @ swap h-front-face !
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
: hit-sphere ( sphere ray rec vp  -- flag ) ( f-t-min f-t-max -- )
  locals| vp rec ray s |
  vp vec3-zero locals| oc |
  vp vec3-zero locals| dir |
  ray r-origin s s-center oc v- 
  ray r-direction dir vec3-move
  dir vlength2  \ tmin tmax a
  dir oc vdot \ tmin tmax a b/2
  oc vlength2 s s-radius f@ fdup f* f- \ tmin tmax a b/2 c
  1 fpick fdup f* \ tmin tmax a b/2 c (b/2)^2 
  3 fpick 2 fpick f* \ tmin tmax a b/2 c (b/2)^2 ac
  f- \ tm6in tmax a b/2 c d
  oc vp pool-free
  dir vp pool-free
  fdup f0< if
    fdrop fdrop fdrop fdrop fdrop fdrop false \ no hit
  else
    fsqrt fdup \ tmin tmax a b/2 c sqrt(d) sqrt(d)
    3 fpick fnegate fswap f- \ tmin tmax a b/2 c sqrt(d) t1
    4 fpick f/ \ tmin tmax a b/2 c sqrt(d) t2
    fdup fdup \ tmin tmax a b/2 c sqrt(d) t2 t2 t2
    8 fpick fswap f< 6 fpick f< and if
      fdup rec h-t-val f! \ tmin tmax a b/2 c sqrt(d) t2
      vp vec3-zero locals| at |
      ray at vp ray-at
      at rec h-point vec3-move
      at s s-center v-= at s s-radius f@ vdiv=
      ray at rec hit-record-set-face-normal
      fdrop fdrop fdrop fdrop fdrop fdrop
      true
      at vp pool-free
      exit
    then
    fdrop \ tmin tmax a b/2 c sqrt(d)
    fdup \ tmin tmax a b/2 c sqrt(d) sqrt(d)
    3 fpick fnegate fswap f+ \ tmin tmax a b/2 c sqrt(d) t1
    4 fpick f/ \ tmin tmax a b/2 c sqrt(d) t2
    fdup fdup \ tmin tmax a b/2 c sqrt(d) t2 t2 t2
    8 fpick fswap f< 6 fpick f< and if
      fdup rec h-t-val f! \ tmin tmax a b/2 c sqrt(d)
      vp vec3-zero locals| at |
      ray at vp ray-at
      at rec h-point vec3-move
      at s s-center v-= at s s-radius f@ vdiv=
      ray at rec hit-record-set-face-normal
      fdrop fdrop fdrop fdrop fdrop fdrop
      true
      at vp pool-free
      exit
    then
    fdrop fdrop fdrop fdrop fdrop fdrop fdrop
    false
  then
;

\ Tests
: test-sphere ( -- )
  cr ." ---test-sphere" cr
  1024 arena-create locals| arena |
  arena vec3-pool-create locals| vp |
  arena ray-pool-create locals| rp |
  arena sphere-pool-create locals| sp |
  arena hit-record-pool-create locals| hrp |
  
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
  hrp hit-record-empty locals| hr |
  0.001e 1000e sphere r hr vp hit-sphere if
    ." Ray hits sphere!" cr
    hr .hit-record
  else
    ." Ray misses sphere." cr
  then

  \ Free resources
  sphere sp pool-free
  center vp pool-free
  o vp pool-free
  d vp pool-free
  r hrp pool-free

  check-stacks
;