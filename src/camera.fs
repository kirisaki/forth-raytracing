begin-structure camera%
  point3% +field origin
  point3%   +field lower-left-corner
  vec3%   +field horizontal
  vec3%   +field vertical
end-structure

\ Initialize camera
: camera-init! ( addr o llc h v -- addr )
  >r >r >r >r
  dup origin    r> swap vec3-move 
  dup lower-left-corner r> swap vec3-move
  dup horizontal r> swap vec3-move
  dup vertical   r> swap vec3-move
;

\ New camera
: camera-new ( o llc h v -- addr )
  camera% allocate throw
  rot rot rot rot
  camera-init!
; 

\ Allocate a camera and initialize it
: camera-alloc ( o llc h v "name" -- )
  create
    camera-new ,
  does> ( -- addr ) @
;

\ Make a default camera
: default-camera ( -- addr )
  0e 0e 0e vec3-new locals| origin |
  3.5555556e 0e 0e vec3-new locals| horizontal |
  0e 2e 0e vec3-new locals| vertical |
  origin
  horizontal 2e vdiv v-
  vertical 2e vdiv v-
  0e 0e 1e vec3-new v- locals| lower-left-corner |
  origin lower-left-corner horizontal vertical
  camera-new
;

\ Free a camera
: camera-free ( addr -- ) free throw ;

\ Display a camera
: .camera ( c -- )
  ." Camera:" cr
  s" origin: " type cr
  dup origin .v cr
  s" lower-left-corner: " type cr
  dup lower-left-corner .v cr
  s" horizontal: " type cr
  dup horizontal .v cr
  s" vertical: " type cr
  vertical .v cr
  cr
;

\ Get ray from camera at (u,v)
: get-ray ( c -- ray ) ( u v -- )
  locals| cam |
  cam origin
  cam lower-left-corner
  cam horizontal vmul v+
  cam vertical vmul v+
  cam origin v-
  ray-new
;