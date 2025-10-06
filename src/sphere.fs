\ Determine if a ray hits a sphere
: hit-sphere ( vec-center ray vp -- flag ) ( radius -- )
  locals| vp ray center |
  vp vec3-zero locals| oc |
  ray r-origin center oc v-
  oc ray r-direction vdot 2e f* fdup f*
  oc oc vdot 2 fpick fdup f* f- 4e f* ray r-direction dup vdot f* f-
  f0>
  fdrop
  oc vp pool-free
;