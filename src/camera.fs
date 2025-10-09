begin-structure camera%
  vec3% +field c-origin
  vec3% +field c-llc
  vec3% +field c-horizontal
  vec3% +field c-vertical
end-structure

\ Initialize camera pool
: camera-pool-create ( arena -- pool )
  camera% 8 pool-init
;

\ Create new camera
: camera-new ( o llc h v cp -- addr )
  locals| cp v h llc o |
  cp pool-alloc
  dup c-origin    o swap vec3-move
  dup c-llc       llc swap vec3-move
  dup c-horizontal h swap vec3-move
  dup c-vertical   v swap vec3-move 
;

\ Make a default camera
: default-camera ( vp cp -- cam )
  locals| cp vp |
  vp vec3-zero locals| tmp |
  vp vec3-zero locals| h-tmp |
  vp vec3-zero locals| v-tmp |
  0e 0e 1e vp vec3-new locals| llc |
  0e 0e 0e vp vec3-new locals| orig |
  3.5555556e 0e 0e vp vec3-new locals| horizontal |
  0e 2e 0e vp vec3-new locals| vertical |
  orig tmp vec3-move
  horizontal 2e h-tmp vdiv
  vertical 2e v-tmp vdiv
  tmp h-tmp v-= tmp v-tmp v-=
  tmp llc v-=

  cp pool-alloc
  dup c-origin orig swap vec3-move
  dup c-horizontal horizontal swap vec3-move
  dup c-vertical vertical swap vec3-move
  dup c-llc tmp swap vec3-move

  tmp vp pool-free
  h-tmp vp pool-free
  v-tmp vp pool-free
  orig vp pool-free
  horizontal vp pool-free
  vertical vp pool-free
  llc vp pool-free
;

\ Make a camera
: make-camera ( lookfrom lookat vup vp cp  -- cam-addr ) ( fov aspect -- )
  locals| cp vp vup lookat lookfrom |
  fswap 2e f/ ftan \ h
  2e f* \ aspect height
  ftuck f* \ height width

  vp vec3-zero vp vec3-zero vp vec3-zero locals| u v w |
  vp vec3-zero vp vec3-zero locals| h/2 v/2 |
  vp vec3-zero locals| llc |

  lookfrom lookat w v- w vunit=
  vup w u vcross u vunit=
  w u v vcross
  u vmul=
  v vmul=
  u h/2 2e vdiv
  v v/2 2e vdiv
  lookfrom llc vec3-move
  llc h/2 v-=
  llc v/2 v-= 
  llc w v-=
  
  lookfrom llc u v cp camera-new
  u vp pool-free
  v vp pool-free
  w vp pool-free
  h/2 vp pool-free
  v/2 vp pool-free
  llc vp pool-free
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
: get-ray ( c out-ray vp -- ) ( u v -- )
  locals| vp ray-out cam |
  vp vec3-zero vp vec3-zero vp vec3-zero locals| dir tmp-h tmp-v |

  cam c-llc dir vec3-move
  cam c-vertical tmp-v vmul
  cam c-horizontal tmp-h vmul
  dir tmp-v v+=
  dir tmp-h v+=
  dir cam c-origin v-=

  cam c-origin ray-out r-origin vec3-move
  dir ray-out r-direction vec3-move

  dir vp pool-free
  tmp-h vp pool-free
  tmp-v vp pool-free
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