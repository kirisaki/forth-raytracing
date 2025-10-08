begin-structure material%
  field: m-type
  vec3% +field m-albedo
  ffield: m-fuzz
end-structure

\ Material types
0 constant lambertian
1 constant metal
2 constant dielectric

\ Initialize material pool
: material-pool-create ( arena -- pool )
  material% 8 pool-init
;

\ New material
: material-new ( type albedo mp -- addr ) ( f-fuzz -- )
  pool-alloc >r
  r@ m-albedo vec3-move
  r@ m-type !
  r@ m-fuzz f!
  r>
;

\ Free a material
: material-free ( addr -- ) free throw ;

\ Display a material
: .material ( m -- )
  ." Material:" cr
  s" type: " type cr
  dup m-type @ . cr
  s" albedo: " type cr
  dup m-albedo .v cr
  s" fuzz: " type cr cr
  m-fuzz f@ f. cr
  cr
;

: scatter ( mat ray rec ray-out att-aout vp rng -- flag rng ) 
  locals| gen vp att-out ray-out rec ray mat |
  mat m-type @
  case
    lambertian of
      vp vec3-zero locals| r |
      gen r vrand-in-unit-sphere to gen
      r rec h-normal v+=
      rec h-point ray-out r-origin vec3-move
      r ray-out r-direction vec3-move
      mat m-albedo att-out vec3-move
      true
      gen
      r vp pool-free
    endof
    metal of
      vp vec3-zero vp vec3-zero vp vec3-zero locals| r reflected rndv |
      ray r-direction r vunit
      r rec h-normal reflected vreflect
      gen rndv vrand-in-unit-sphere to gen
      rndv mat m-fuzz f@ vmul=
      reflected rndv v+=
      rec h-point ray-out r-origin vec3-move
      reflected ray-out r-direction vec3-move
      mat m-albedo att-out vec3-move
      ray-out r-direction rec h-normal vdot f0>
      gen
      r vp pool-free
      reflected vp pool-free
      rndv vp pool-free
    endof
  endcase
;

\ Tests
: test-material ( -- )
  s" ---material test" type cr
  1024 arena-create locals| arena |
  arena material-pool-create locals| mp |
  arena vec3-pool-create locals| vp |

  s" Display material:" type cr
  metal 0.5e 0.3e 0.2e vp vec3-new 0.0e mp material-new locals| m |
  m .material

  check-stacks
;