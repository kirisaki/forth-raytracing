begin-structure material%
  field: material-type
  field: albedo
  ffield: fuzz
end-structure

\ Material types
0 constant lambertian
1 constant metal
2 constant dielectric

\ Initialize material
: material-init! ( addr type albedo -- addr ) ( f-fuzz -- )
  locals| al ty addr |
  ty addr material-type !
  al addr albedo !
  addr fuzz f!
  addr
;

\ New material
: material-new ( type albedo -- addr ) ( f-fuzz -- )
  >r >r
  material% allocate throw
  r> r>
  material-init!
;

\ Allocate a material and initialize it
: material-alloc ( type "name" -- ) ( f-albedo f-fuzz -- )
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
  cr
;

\ Scatter ray
: scatter ( mat ray rec rng -- out-ray att flag rng ) 
  locals| rng rec ray mat |
  mat material-type @ @
  case
    lambertian of
      rec normal @ rng vrand-in-unit-sphere swap >r v+
      rec point @ swap ray-new
      mat @ albedo @
      true
      r>
    endof
    metal of
      ray direction vunit
      rec normal @ vreflect 
      rec point @ swap ray-new dup
      mat @ albedo @
      swap direction rec normal @ vdot f0>
      rng
    endof
  endcase
;

: test-material ( -- )
  s" ---material test---" type cr
  metal 0.2e 0.3e 0.4e color-new 0.3e material-new locals| m |
  m .material
;