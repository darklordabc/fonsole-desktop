<template>
  <div id="app">
    <body>
      <rings></rings>
      <div id="appContent">
        <div id="topbarContainer" style="box-shadow: 0 0 1vh rgba(0,0,0,0.5);">
          <topbar></topbar>
        </div>
        <div id="routerContainer">
          <transition name="fade" mode="out-in">
            <component
              :is="page"
            ></component>
          </transition>
        </div>
      </div>
    </body>
  </div>
</template>

<script>
  import Rings from '@/components/Rings/Rings';
  import Topbar from '@/components/Topbar/Topbar';
  import About from '@/components/About/About';
  import Game from '@/components/Game/Game';
  import Home from '@/components/Home/Home';
  import Games from '@/components/Games/Games';
  import GameView from '@/components/Games/GameView';
  import Shop from '@/components/Shop/Shop';
  import Community from '@/components/Community/Community';
  import Settings from '@/components/Settings/Settings';
// @ifdef ELECTRON
  import Workshop from '@/components/Workshop/Workshop';
  import Workspace from '@/components/Workspace/Workspace';
// @endif

  export default {
    name: 'App',
    components: {
      rings: Rings,
      topbar: Topbar,
      about: About,
      game: Game,
      home: Home,
      games: Games,
      gameView: GameView,
      shop: Shop,
      community: Community,
      settings: Settings,
// @ifdef ELECTRON
      Workshop,
      Workspace,
// @endif
    },
    computed: {
      page() {
        return this.$store.state.gui.page;
      },
    },
  };
/* eslint-disable max-len */
</script>

<style lang="sass">
  @font-face
    font-family: 'zekton'
    src: url('./assets/zekton.ttf') format('truetype')
    font-weight: normal
    font-style: normal

  $font-stack:    zekton, Helvetica, sans-serif
  $primary-color: white

  $dividerColor: rgba(255, 255, 255, 0.65)
  $dividerColorTransparent: rgba(255, 255, 255, 0)

  body
    margin: 0
    padding: 0
    overflow: hidden
    background-color: #222
    color: $primary-color
    font-family: 'zekton'
    text-transform: uppercase

  label, h1, h2, div
    cursor: default
    user-select: none
    font-weight: 100

  #appContent
    height: 100%
    width: 100%
    position: absolute
    display: flex
    flex-direction: column

  #topbarContainer
    flex: 0 1 auto
    background-color: rgba(20,20,20,.8)

  #routerContainer
    flex: 1 1 auto
    margin: 2vh
    display: flex
    flex-direction: column

    .tabContents
      flex: 1 1 auto
      margin: 0px
      box-shadow: 0 0 1vh rgba(0,0,0,0.5)

  .divider
    margin-top: 6px
    margin-bottom: 6px
    margin-left: 1px
    width: 100%
    height: 0.20vh
    background: linear-gradient(to right, $dividerColor 0%, $dividerColor 5%, $dividerColor 50%, $dividerColorTransparent 95%, $dividerColorTransparent 100%)

  .fade-enter-active, .fade-leave-active
    transition: all .3s

  .fade-enter, .fade-leave-to
    transform: translateY(4vh)
    opacity: 0
</style>
