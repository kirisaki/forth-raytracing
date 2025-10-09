begin-structure camera%
  vec3% +field c-origin
  vec3% +field c-llc
  vec3% +field c-horizontal
  vec3% +field c-vertical
  vec3% +field c-u
  vec3% +field c-v
  vec3% +field c-w
  ffield: c-lens-radius
end-structure

\ Initialize camera pool
: camera-pool-create ( arena -- pool )
  camera% 8 pool-init
;

\ Create new camera
: camera-new ( o llc hor ver u v w cp -- addr ) ( f-lens-radius -- )
  locals| cp w v u ver hor llc o |
  cp pool-alloc
  dup c-origin    o swap vec3-move
  dup c-llc       llc swap vec3-move
  dup c-horizontal hor swap vec3-move
  dup c-vertical   ver swap vec3-move 
  dup c-u         u swap vec3-move
  dup c-v         v swap vec3-move
  dup c-w         w swap vec3-move
  dup c-lens-radius f!
;

\ Make a camera
: make-camera ( lookfrom lookat vup vp cp  -- cam-addr ) ( fov aspect aperture focus-dist -- )
  locals| cp vp vup lookat lookfrom |
  3 fpick 2e f/ ftan 
  2e f*
  fdup 4 fpick f* \ fov aspect aperture focus-dist height width

  vp vec3-zero vp vec3-zero vp vec3-zero locals| u v w |
  vp vec3-zero vp vec3-zero locals| hori vert |
  vp vec3-zero vp vec3-zero locals| h/2 v/2 |
  vp vec3-zero locals| llc |

  lookfrom lookat w v- w vunit=
  vup w u vcross u vunit=
  w u v vcross
  u 2 fpick f* hori vmul
  v fover f* vert vmul
  vp vec3-zero locals| w' |
  w w' vmul
  hori h/2 2e vdiv
  vert v/2 2e vdiv
  lookfrom llc vec3-move
  llc h/2 v-=
  llc v/2 v-= 
  llc w' v-=
  
  lookfrom llc hori vert u v w 2e f/ cp camera-new
  u vp pool-free
  v vp pool-free
  w vp pool-free
  w' vp pool-free
  h/2 vp pool-free
  v/2 vp pool-free
  llc vp pool-free
  fdrop fdrop
;

\ Display a camera
: .camera ( c -- )
  ." Camera:" cr
  s" origin: " type cr
  dup c-origin .v cr
  s" lower-left-corner: " type cr
  dup c-llc .v cr
  s" horizontal: " type cr
  dup c-horizontal .v cr
  s" vertical: " type cr
  c-vertical .v cr
  cr
;

\ Get ray from camera at (u,v)
: get-ray ( cam out-ray gen vp -- gen ) ( s t -- )
  locals| vp gen ray-out cam |
  vp vec3-zero vp vec3-zero vp vec3-zero locals| dir tmp-h tmp-v |
  vp vec3-zero vp vec3-zero locals| rd rnd |
  gen rnd vp vrand-in-unit-disk to gen
  cam c-lens-radius f@ rnd rd vmul
  vp vec3-zero locals| offset |
  vp vec3-zero vp vec3-zero locals| u' v' |
  cam c-u rd vx f@ u' vmul
  cam c-v rd vy f@ v' vmul
  u' v' offset v+

  cam c-llc dir vec3-move
  cam c-vertical tmp-v vmul
  cam c-horizontal tmp-h vmul
  dir tmp-v v+=
  dir tmp-h v+=
  dir cam c-origin v-=
  dir offset v-=

  offset cam c-origin v+=
  offset ray-out r-origin vec3-move
  dir ray-out r-direction vec3-move

  dir vp pool-free
  tmp-h vp pool-free
  tmp-v vp pool-free
  rd vp pool-free
  rnd vp pool-free
  offset vp pool-free
  u' vp pool-free
  v' vp pool-free
  gen
;

\ Tests
: test-camera ( -- )
  cr ." ---test-camera" cr
  1024 arena-create locals| arena |
  arena vec3-pool-create locals| vp |
  arena camera-pool-create locals| cp |
  arena ray-pool-create locals| rp |

  0e 0e 0e vp vec3-new locals| o |
  0e 0e -1e vp vec3-new locals| llc |
  4e 0e 0e vp vec3-new locals| h |
  0e 2e 0e vp vec3-new locals| v |
  o llc h v cp camera-new locals| cam |
  o o rp ray-new locals| ray |

  cam .camera
  0.7e 0.6e cam ray vp get-ray 
  ray .ray

  arena arena-destroy
  check-stacks
;