begin-structure camera%
  point3% +field cam-origin
  point3%   +field lower-left-corner
  vec3%   +field horizontal
  vec3%   +field vertical
  vec3%   +field cam-u
  vec3%   +field cam-v
  vec3%   +field cam-w
  ffield: lens-radius
end-structure

\ Initialize camera
: camera-init! ( addr o llc horizontal vertical u v w -- addr ) ( f-lens-radius -- )
  >r >r >r >r >r >r >r
  dup cam-origin    r> swap vec3-move 
  dup lower-left-corner r> swap vec3-move
  dup horizontal r> swap vec3-move
  dup vertical   r> swap vec3-move
  dup cam-u r> swap vec3-move
  dup cam-v r> swap vec3-move
  dup cam-w r> swap vec3-move
  dup lens-radius f!
;

\ New camera
: camera-new ( o llc horizontal vertical u v w -- addr ) ( lens-radius -- )
  locals| w v u vertical horizontal llc o |
  camera% allocate throw
  o llc horizontal vertical u v w camera-init!
; 

\ Make a camera
: make-camera ( lookfrom lookat vup -- cam-addr ) ( fov aspect aperture focus-dist -- )
  locals| vup lookat orig |
  3 fpick 2e f/ ftan 
  2e f*
  fdup 4 fpick f* \ fov aspect aperture focus-dist height width

  orig lookat v- vunit \ w
  dup vup swap vcross vunit \ w u
  2dup vcross locals| v u w |

  u 2 fpick f* vmul locals| horizontal | 
  v fover f* vmul locals| vertical |
  w vmul orig swap v- horizontal 2e vdiv v- vertical 2e vdiv v- locals| llc |
  orig llc horizontal vertical u v w 2e f/ camera-new
  fdrop fdrop
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

\ Get ray from camera at (s, t)
: get-ray ( c rng -- ray rng ) ( s t -- )
  >r
  locals| cam |
  cam lens-radius f@ r> vrand-in-unit-disk swap >r vmul locals| rd |
  cam cam-u rd vx f@ vmul
  cam cam-v rd vy f@ vmul
  over over v+ locals| offset | free throw free throw

  cam origin offset v+
  cam lower-left-corner
  cam horizontal fswap vmul v+
  dup cam vertical vmul v+ swap free throw
  dup cam origin v- swap free throw
  dup offset v- swap free throw
  ray-new r>
;