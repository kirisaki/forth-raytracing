begin-structure material%
  field: material-type
  field: albedo
  ffield: fuzz
  ffield: ref-index
end-structure

\ Material types
0 constant lambertian
1 constant metal
2 constant dielectric

\ Initialize material
: material-init! ( addr type albedo -- addr ) ( f-fuzz f-ref-index -- )
  locals| al ty addr |
  ty addr material-type !
  al addr albedo !
  addr fuzz fswap f!
  addr ref-index f!
  addr
;

\ New material
: material-new ( type albedo -- addr ) ( f-fuzz f-ref-index -- )
  >r >r
  material% allocate throw
  r> r>
  material-init!
;

\ Allocate a material and initialize it
: material-alloc ( type "name" -- ) ( f-albedo f-fuzz f-ref-index -- )
  create
    material-new ,
  does> ( -- addr ) @
;

\ Free a material
: material-free ( addr -- ) free throw ;

\ Display a material
: .material ( m -- )
  ." Material:" cr
  s" type: " type cr
  dup material-type @ . cr
  s" albedo: " type cr
  dup albedo @ .v cr
  s" fuzz: " type cr cr
  fuzz f@ f. cr
  s" ref-index: " type cr
  ref-index f@ f. cr
  cr
;

\ Schlieren approximation for reflectance
: schlick ( -- ) ( cos ref-index -- reflectance )
  fdup 1e fswap f- fswap 1e f+ f/ \ cos r0
  fdup f* \ cos r0^2
  fdup 1e fswap f- 1e 3 fpick f- 5e f** f* f+ fnip
;

\ Scatter ray
: scatter ( mat ray rec rng -- out-ray att flag rng ) 
  locals| rng rec ray mat |
  mat material-type @ @
  rng >r
  case
    lambertian of
      rec normal @ r> vrand-in-unit-sphere swap >r tuck v+ swap free throw
      rec point @ swap ray-new
      mat @ albedo @
      true
    endof
    metal of
      ray direction vunit
      rec normal @ vreflect  dup r> vrand-in-unit-sphere swap >r dup mat @ fuzz f@ vmul swap free throw dup rot v+ rot rot free throw free throw
      rec point @ swap ray-new dup
      mat @ albedo @
      swap direction rec normal @ vdot f0>
    endof
    dielectric of
      rec front-face @ if
        1e mat @ ref-index f@ f/ 
      else
        mat @ ref-index f@ 
      then
      fdup
      ray direction vunit -1e vmul rec normal @ vdot 1e fmin \ etai etai cos
      fdup fdup f* 1e fswap f- fabs fsqrt \ etai etai cos sin
      2 fpick f* 1e f> if \ etai etai cos
        \ total internal reflection
        ray direction vunit dup rec normal @ vreflect swap free throw
        rec point @ swap ray-new
        1e 1e 1e color-new
        true
        fdrop fdrop fdrop
      else
        fswap schlick r> frand >r f> if
          \ reflect
          ray direction vunit dup rec normal @ vreflect swap free throw
          rec point @ swap ray-new
          1e 1e 1e color-new
          true
          fdrop 
        else
          \ refraction
          ray direction vunit dup rec normal @ vrefract swap free throw
          rec point @ swap ray-new
          1e 1e 1e color-new
          true
        then
      then
    endof
  endcase
  r>
;

: test-material ( -- )
  s" ---material test---" type cr
  metal 0.2e 0.3e 0.4e color-new 0.3e material-new locals| m |
  m .material
;