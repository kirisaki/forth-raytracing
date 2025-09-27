begin-structure camera%
  point3% +field cam-origin
  point3%   +field lower-left-corner
  vec3%   +field horizontal
  vec3%   +field vertical
end-structure

\ Initialize camera
: camera-init! ( addr o llc h v -- addr )
  >r >r >r >r
  dup cam-origin    r> swap vec3-move 
  dup lower-left-corner r> swap vec3-move
  dup horizontal r> swap vec3-move
  dup vertical   r> swap vec3-move
;

\ New camera
: camera-new ( o llc h v -- addr )
  locals| v h llc o |
  camera% allocate throw
  o llc h v camera-init!
; 

\ Allocate a camera and initialize it
: camera-alloc ( o llc h v "name" -- )
  create
    camera-new ,
  does> ( -- addr ) @
;

\ Make a camera
: make-camera ( lookfrom lookat vup -- cam-addr ) ( fov aspect -- )
  locals| vup lookat orig |
  fswap 2e f/ ftan \ h
  2e f* \ aspect height
  ftuck f* \ height width

  orig lookat v- vunit \ w
  dup vup swap vcross vunit \ w u
  2dup vcross \ w u v
  over vmul locals| horizontal | \ w u v
  vmul locals| vertical | \ w u
  drop orig swap v- horizontal 2e vdiv v- vertical 2e vdiv v- locals| llc |
  orig llc horizontal vertical camera-new
;

\ Free a camera
: camera-free ( addr -- ) free throw ;

\ Display a camera
: .camera ( c -- )
  ." Camera:" cr
  s" origin: " type cr
  dup cam-origin .v cr
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
  cam lower-left-corner
  cam vertical vmul v+
  cam horizontal vmul v+
  cam cam-origin v-
  cam cam-origin swap ray-new
;