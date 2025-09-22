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
  ray-init!
;

\ Initialize hit-record
: hit-record-init! ( addr point normal flag -- addr ) ( f-t -- )
  >r >r >r
  dup point    r> swap vec3-move 
  dup normal   r> swap vec3-move
  dup t-val    f!
  dup front-face r> swap !
;

\ New hit-record
: hit-record-new ( point normal flag -- addr ) ( f-t -- )
  hit-record% allocate throw
  rot rot rot
  hit-record-init!
;

\ Empty hit-record
: hit-record-empty ( -- addr )
  0e 0e 0e vec3-new
  0e 0e 0e vec3-new
  false
  -1e hit-record-new
;

\ Set face normal
: set-face-normal ( ray outward rec -- )
  locals| ray outward rec |
  ray direction outward vdot f0< dup if
    true rec front-face !
  else
    false rec front-face !
    rec normal -1e negate vmul rec normal !
  then
;

\ Hit sphere
: hit-sphere ( sphere ray hit-record -- ) ( f-t-min f-t-max -- flag )
  locals| rec ray s |
  ray origin center v- locals| oc |
  ray direction locals| dir |
  dir vlength2 \ tmin tmax a
  oc dir vdot \ rmin tmax a b/2
  oc vlength2 2 fpick fdup f* f- \ tmin tmax a b/2 c
  1 fpick fdup f* \ tmin tmax a b/2 c (b/2)^2 
  3 fpick 2 fpick f* \ tmin tmax a b/2 c b^2 ac
  f- \ tmin tmax a b/2 c d
  fdup f0< if
    4 0 do fdrop loop false \ no hit
  else
    fsqrt fdup \ tmin tmax a b/2 c sqrt(d) sqrt(d)
    3 fpick fswap f- 4 fpick f/ \ tmin tmax a b/2 c sqrt(d) t1
    fdup 7 fpick f< 6 fpick fswap f< or if
      3 fpick f+ 4 fpick f/ \ tmin tmax a b/2 c sqrt(d) t2
      fdup 7 fpick f< 6 fpick fswap f< or if
        false
      then
    then
    rec t-val f!
    ray at dup rec point !
    center v- s radius f@ vdiv
    ray swap rec set-face-normal
    true
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