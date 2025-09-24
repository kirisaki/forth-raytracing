begin-structure sphere%
  point3% +field center
  ffield: radius
end-structure

begin-structure hit-record%
  point3% +field point
  vec3%   +field normal
  ffield: t-val
  field: front-face
end-structure

\ Initialize sphere
: sphere-init! ( addr center -- addr ) ( f-radius -- )
  >r
  dup center r> swap vec3-move 
  dup radius f!
;

\ New sphere
: sphere-new ( center -- addr ) ( f-radius -- )
  sphere% allocate throw
  swap
  sphere-init!
;

\ Display sphere
: .sphere ( s -- )
  s" center: " type cr
  dup center .v cr
  s" radius: " type cr
  radius f@ f. cr
  cr
;

\ Initialize hit-record
: hit-record-init! ( addr point normal flag -- addr ) ( f-t -- )
  >r >r >r
  dup point    r> swap !
  dup normal   r> swap !
  dup t-val    f!
  dup front-face r> swap !
;

\ New hit-record
: hit-record-new ( point normal flag -- addr ) ( f-t -- )
  >r >r >r
  hit-record% allocate throw
  r> r> r>
  hit-record-init!
;

\ Empty hit-record
: hit-record-empty ( -- addr )
  0e 0e 0e vec3-new
  0e 0e 0e vec3-new
  false
  -1e hit-record-new
;

\ Display hit-record
: .hit-record ( rec -- )
  s" point: " type cr
  dup point @ .v cr
  s" normal: " type cr
  dup normal @ .v cr
  s" t-val: " type cr
  dup t-val f@ f. cr
  s" front-face: " type cr
  front-face @ if ." true" else ." false" then cr
  cr
;

\ Set face normal
: set-face-normal ( ray outward rec -- )
  locals| rec outward ray |
  ray direction outward vdot f0< dup if
    rec front-face !
    outward rec normal !
  else
    rec front-face !
    outward -1e vmul rec normal !
  then
;

\ Hit sphere
: hit-sphere ( sphere ray rec -- flag ) ( f-t-min f-t-max -- )
  locals| rec ray s |
  ray origin s center v- locals| oc |
  ray direction locals| dir |
  dir vlength2 \ tmin tmax a
  dir oc vdot \ tmin tmax a b/2
  oc vlength2 s radius f@ fdup f* f- \ tmin tmax a b/2 c
  1 fpick fdup f* \ tmin tmax a b/2 c (b/2)^2 
  3 fpick 2 fpick f* \ tmin tmax a b/2 c (b/2)^2 ac
  f- \ tm6in tmax a b/2 c d
  fdup f0< if
    fdrop fdrop fdrop fdrop fdrop fdrop false \ no hit
  else
    fsqrt fdup \ tmin tmax a b/2 c sqrt(d) sqrt(d)
    3 fpick fnegate fswap f- \ tmin tmax a b/2 c sqrt(d) t1
    4 fpick f/ \ tmin tmax a b/2 c sqrt(d) t2
    fdup fdup \ tmin tmax a b/2 c sqrt(d) t2 t2 t2
    8 fpick fswap f< 6 fpick f< and if
      fdup rec t-val f! \ tmin tmax a b/2 c sqrt(d) t2
      ray at dup rec point !
      s center v- s radius f@ vdiv
      ray swap rec set-face-normal
      fdrop fdrop fdrop fdrop fdrop fdrop
      true
      exit
    then
    fdrop \ tmin tmax a b/2 c sqrt(d)
    fdup \ tmin tmax a b/2 c sqrt(d) sqrt(d)
    3 fpick fnegate fswap f+ \ tmin tmax a b/2 c sqrt(d) t1
    4 fpick f/ \ tmin tmax a b/2 c sqrt(d) t2
    fdup fdup \ tmin tmax a b/2 c sqrt(d) t2 t2 t2
    8 fpick fswap f< 6 fpick f< and if
      fdup rec t-val f! \ tmin tmax a b/2 c sqrt(d)
      ray at dup rec point !
      s center v- s radius f@ vdiv
      ray swap rec set-face-normal
      fdrop fdrop fdrop fdrop fdrop fdrop
      true
      exit
    then
    fdrop fdrop fdrop fdrop fdrop fdrop fdrop
    false
  then
;

: test-sphere ( -- )
  s" ---sphere test" type cr
  1e 2e 3e vec3-new 1e sphere-new
  locals| s |

  s" center" type cr
  s center .v cr
  s" radius" type cr
  s radius f@ f. cr
  cr

;