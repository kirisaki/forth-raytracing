: hit-sphere ( center ray vp -- ) ( f-radius -- f )
  locals| vp ray center |
  vp vec3-zero locals| oc |
  ray r-origin center oc v- 
  vp vec3-zero locals| dir |
  ray r-direction dir vec3-move 
  dir vlength2 \ r a
  oc dir vdot \ r a b/2
  oc vlength2 3 fpick fdup f* f- \ r a b/2 c
  1 fpick fdup f* \ r a b/2 c (b/2)^2 
  3 fpick 2 fpick f* \ r a b/2 c b^2 ac
  f- \ r a b/2 c d
  fdup f0< if
    5 0 do fdrop loop -1e \ no hit
  else
    fsqrt
    2 fpick fnegate fswap f- 3 fpick f/
    4 0 do fdrop loop
  then
  oc vp pool-free
  dir vp pool-free
;