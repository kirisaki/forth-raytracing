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
: default-camera ( out vp -- )
  locals| vp out |
  vp vec3-zero locals| h-tmp |
  vp vec3-zero locals| v-tmp |
  0e 0e 0e vp vec3-new locals| orig |
  3.5555556e 0e 0e vp vec3-new locals| horizontal |
  0e 2e 0e vp vec3-new locals| vertical |
  orig out vec3-move
  horizontal 2e h-tmp vdiv
  vertical 2e v-tmp vdiv
  out h-tmp v-= out v-tmp v-=
  0e 0e 1e vp vec3-new locals| llc |

  out c-origin vec3-move
  out c-horizontal horizontal vec3-move
  out c-vertical vertical vec3-move
  out c-llc llc vec3-move
  out

  h-tmp vp pool-free
  v-tmp vp pool-free
  orig vp pool-free
  horizontal vp pool-free
  vertical vp pool-free
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
  locals| vp out cam |
  vp vec3-zero vp vec3-zero locals| tmp-h tmp-v |
  cam c-llc out vec3-move
  cam c-vertical tmp-v vmul
  cam c-horizontal tmp-h vmul
  out tmp-v v+=
  out tmp-h v+=
  out cam c-origin v-=
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