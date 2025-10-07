\ Detect ray-object intersections in a list of hittable objects
: hit ( ray head rec hrp vp -- flag ) ( tmin tmax -- )
  locals| vp hrp rec head ray |
  ray
  1 cells allocate throw locals| hit-anything |
  false hit-anything !
  hit-anything
  rec
  hrp
  vp
  head
  [: ( ray hit-anything rec hrp vp sphere -- ) ( tmin tmax -- )
    fover fover
    locals| sphere vp hrp rec hit-anything ray |
    hrp hit-record-empty locals| temp-rec |
    ray hit-anything rec hrp vp \ For next iteration
    sphere ray temp-rec vp hit-sphere if
      true hit-anything !
      fdrop temp-rec h-t-val f@
      temp-rec rec hit-record% move
    then
    temp-rec hrp pool-free
  ;] foreach
  2drop 2drop drop
  fdrop fdrop 
  hit-anything @
  hit-anything free throw
;

\ Tests
: test-hit ( -- )
  cr ." ---test-hit" cr

  1024 arena-create locals| arena |
  arena sphere% 8 pool-init locals| sp |
  arena ray% 8 pool-init locals| rp |
  arena vec3% 8 pool-init locals| vp |
  arena hit-record% 8 pool-init locals| hrp |

  \ Create a sphere
  0e 0e -1e vp vec3-new locals| center |
  0.5e center sp sphere-new locals| sphere |
  
  \ Create a list of spheres
  0 locals| head |
  sphere head sp push-front to head

  \ Create a ray
  0e 0e 0e vp vec3-new locals| origin |
  0e 0e -1e vp vec3-new locals| direction |
  origin direction rp ray-new locals| ray |

  \ Create hit record
  hrp hit-record-empty locals| hr |

  \ Test hit
  0.001e 1000e ray head hr hrp vp hit if
    ." Ray hits sphere!" cr
    hr .hit-record
  else
    ." Ray misses sphere." cr
  then

  check-stacks
;