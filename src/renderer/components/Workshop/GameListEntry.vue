<template>
  <div :class="['game-list-entry', {
    'create-button': isCreateButton,
    selected,
  }]">
    <div class="inline">
      <img :src="coverImage">
    </div>
    <div class="inline content">
      {{ gameName }}
    </div>
  </div>
</template>

<script>
  import { WORKSHOP_PREVIEW_TYPE } from '=/enums';

  export default {
    name: 'GameListEntry',
    props: {
      previewData: [String, Number],
      selected: Boolean,
    },
    computed: {
      isCreateButton() {
        return this.previewData === WORKSHOP_PREVIEW_TYPE.CREATE;
      },
      coverImage() {
        if (this.isCreateButton) {
          return require('@/assets/testgame5.jpg');
        }
        return require('@/assets/testgame2.jpg');
      },
      gameName() {
        if (this.isCreateButton) {
          return this.$localize('create');
        }
        return (this.$store.getters.workshopGameInfo(this.previewData) || {}).name;
      },
    },
  };
</script>

<style lang="sass" scoped>
  img
    height: 6.575vh
    margin: 0.5vh
    border: 0.25vh solid rgba(25, 25, 25, 0.6)

  .game-list-entry
    box-sizing: border-box
    width: 100%
    height: 8vh
    background-color: rgba(25, 25, 25, 0.6)
    transition: all 0.1s linear
    display: inline-block
    border-bottom: 0.05vh solid rgba(25, 25, 25, 0.6)
    // box-shadow: inset 0px -0.1vh 0 0.1vh rgba(15, 15, 15, 0.2)
    border-left: 1vw none rgba(10, 120, 230, 0.7)

    &.create-button
      background: rgba(72,131,75,0.7)
      box-shadow: inset 0 0 0.5vw rgba(25, 25, 25, 0.6)

    .inline
      vertical-align: middle
      display: inline-block

    .content
      font-size: 3vh

    &:hover,
    &.selected
      border-left: 1vw solid rgba(10, 120, 230, 0.7)

    &:active
      border-left: 1vw none rgba(10, 120, 230, 0.7)

    &.create-button:hover,
    &.create-button.selected
      border-left: 1vw none rgba(10, 120, 230, 0.7)
      opacity: 0.8

    &.create-button:active
      opacity: 1
</style>
