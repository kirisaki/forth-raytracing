begin-structure ray%
  point3% +field origin
  vec3%   +field direction
end-structure

\ Initialize ray
: ray-init! ( addr o d -- addr )
  >r >r
  dup origin    r> swap vec3-move 
  dup direction r> swap vec3-move
;

\ New ray
: ray-new ( o d -- addr )
  ray% allocate throw
  rot rot
  ray-init!
;

\ Allocate a ray and initialize it
: ray-alloc ( o d "name" -- )
  create
    ray-new ,
  does> ( -- addr ) @
;

\ Free a ray
: ray-free ( addr -- ) free throw ;

\ Compute point at parameter t on the ray
: at ( ray -- point3 ) ( f -- )
  dup direction 
  vmul
  origin v+
;

\ Test ray
: test-ray ( -- )
  s" ---ray test" type cr
  0e 0e 0e vec3-new
  10e 20e 30e vec3-new

  s" ray" type cr
  ray-new
  ray% allocate throw
  locals| s r |

  s" origin" type cr
  r origin .v cr
  s" direction" type cr
  r direction .v cr
  s" at 2.0" type cr
  r 2e at
  .v cr
  cr
;