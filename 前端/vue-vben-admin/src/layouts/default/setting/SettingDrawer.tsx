import { defineComponent, computed, unref } from 'vue';
import { BasicDrawer } from '@/components/Drawer';
import { Divider } from 'ant-design-vue';
import {
  TypePicker,
  ThemeColorPicker,
  SettingFooter,
  SwitchItem,
  SelectItem,
  InputNumberItem,
} from './components';

import { AppDarkModeToggle } from '@/components/Application';

import { MenuTypeEnum, TriggerEnum } from '@/enums/menuEnum';

import { useRootSetting } from '@/hooks/setting/useRootSetting';
import { useMenuSetting } from '@/hooks/setting/useMenuSetting';
import { useHeaderSetting } from '@/hooks/setting/useHeaderSetting';
import { useMultipleTabSetting } from '@/hooks/setting/useMultipleTabSetting';
import { useTransitionSetting } from '@/hooks/setting/useTransitionSetting';
import { useI18n } from '@/hooks/web/useI18n';

import { baseHandler } from './handler';

import {
  HandlerEnum,
  contentModeOptions,
  topMenuAlignOptions,
  getMenuTriggerOptions,
  routerTransitionOptions,
  menuTypeListEnum,
  mixSidebarTriggerOptions,
} from './enum';

// import {
//   HEADER_PRESET_BG_COLOR_LIST,
//   SIDE_BAR_BG_COLOR_LIST,
//   APP_PRESET_COLOR_LIST,
// } from '@/settings/designSetting';
import { SIDE_BAR_BG_COLOR_LIST } from '@/settings/designSetting';

const { t } = useI18n();

export default defineComponent({
  name: 'SettingDrawer',
  setup(_, { attrs }) {
    const {
      getContentMode,
      getShowFooter,
      getShowBreadCrumb,
      getShowBreadCrumbIcon,
      getShowLogo,
      getFullContent,
      getColorWeak,
      getGrayMode,
      getLockTime,
      getShowDarkModeToggle,
      // getThemeColor,
    } = useRootSetting();

    const { getOpenPageLoading, getBasicTransition, getEnableTransition, getOpenNProgress } =
      useTransitionSetting();

    const {
      getIsHorizontal,
      getShowMenu,
      getMenuType,
      getTrigger,
      getCollapsedShowTitle,
      getMenuFixed,
      getCollapsed,
      getCanDrag,
      getTopMenuAlign,
      getAccordion,
      getMenuWidth,
      getMenuBgColor,
      getIsTopMenu,
      getSplit,
      getIsMixSidebar,
      getCloseMixSidebarOnChange,
      getMixSideTrigger,
      getMixSideFixed,
    } = useMenuSetting();

    const {
      getShowHeader,
      getFixed: getHeaderFixed,
      // getHeaderBgColor,
      getShowSearch,
    } = useHeaderSetting();

    const { getShowMultipleTab, getShowQuick, getShowRedo, getShowFold, getAutoCollapse } =
      useMultipleTabSetting();

    const getShowMenuRef = computed(() => {
      return unref(getShowMenu) && !unref(getIsHorizontal);
    });

    function renderSidebar() {
      return (
        <>
          <TypePicker
            menuTypeList={menuTypeListEnum}
            handler={(item: (typeof menuTypeListEnum)[0]) => {
              baseHandler(HandlerEnum.CHANGE_LAYOUT, {
                mode: item.mode,
                type: item.type,
                split: unref(getIsHorizontal) ? false : undefined,
              });
            }}
            def={unref(getMenuType)}
          />
        </>
      );
    }

    // function renderHeaderTheme() {
    //   return (
    //     <ThemeColorPicker
    //       colorList={HEADER_PRESET_BG_COLOR_LIST}
    //       def={unref(getHeaderBgColor)}
    //       event={HandlerEnum.HEADER_THEME}
    //     />
    //   );
    // }

    function renderSideBarTheme() {
      return (
        <ThemeColorPicker
          colorList={SIDE_BAR_BG_COLOR_LIST}
          def={unref(getMenuBgColor)}
          event={HandlerEnum.MENU_THEME}
        />
      );
    }

    // function renderMainTheme() {
    //   return (
    //     <ThemeColorPicker
    //       colorList={APP_PRESET_COLOR_LIST}
    //       def={unref(getThemeColor)}
    //       event={HandlerEnum.CHANGE_THEME_COLOR}
    //     />
    //   );
    // }

    /**
     * @description:
     */
    function renderFeatures() {
      let triggerDef = unref(getTrigger);

      const triggerOptions = getMenuTriggerOptions(unref(getSplit));
      const some = triggerOptions.some((item) => item.value === triggerDef);
      if (!some) {
        triggerDef = TriggerEnum.FOOTER;
      }

      return (
        <>
          <SwitchItem
            title={t('layout.setting.menuDrag')}
            event={HandlerEnum.MENU_HAS_DRAG}
            def={unref(getCanDrag)}
            disabled={!unref(getShowMenuRef)}
          />
          <SwitchItem
            title={t('layout.setting.menuSearch')}
            event={HandlerEnum.HEADER_SEARCH}
            def={unref(getShowSearch)}
            disabled={!unref(getShowHeader)}
          />
          <SelectItem
            title={t('layout.setting.topMenuLayout')}
            event={HandlerEnum.MENU_TOP_ALIGN}
            def={unref(getTopMenuAlign)}
            options={topMenuAlignOptions}
            disabled={
              !unref(getShowHeader) ||
              unref(getSplit) ||
              (!unref(getIsTopMenu) && !unref(getSplit)) ||
              unref(getIsMixSidebar)
            }
          />
          <SelectItem
            title={t('layout.setting.menuCollapseButton')}
            event={HandlerEnum.MENU_TRIGGER}
            def={triggerDef}
            options={triggerOptions}
            disabled={!unref(getShowMenuRef) || unref(getIsMixSidebar)}
          />
          <SelectItem
            title={t('layout.setting.contentMode')}
            event={HandlerEnum.CONTENT_MODE}
            def={unref(getContentMode)}
            options={contentModeOptions}
          />
          <InputNumberItem
            title={t('layout.setting.autoScreenLock')}
            min={0}
            event={HandlerEnum.LOCK_TIME}
            defaultValue={unref(getLockTime)}
            formatter={(value: string) => {
              return parseInt(value) === 0
                ? `0(${t('layout.setting.notAutoScreenLock')})`
                : `${value}${t('layout.setting.minute')}`;
            }}
          />
        </>
      );
    }

    function renderContent() {
      return (
        <>
          <SwitchItem
            title={t('layout.setting.tabs')}
            event={HandlerEnum.TABS_SHOW}
            def={unref(getShowMultipleTab)}
          />

          <SwitchItem
            title={t('layout.setting.tabsRedoBtn')}
            event={HandlerEnum.TABS_SHOW_REDO}
            def={unref(getShowRedo)}
            disabled={!unref(getShowMultipleTab)}
          />

          <SwitchItem
            title={t('layout.setting.tabsQuickBtn')}
            event={HandlerEnum.TABS_SHOW_QUICK}
            def={unref(getShowQuick)}
            disabled={!unref(getShowMultipleTab)}
          />
          <SwitchItem
            title={t('layout.setting.tabsFoldBtn')}
            event={HandlerEnum.TABS_SHOW_FOLD}
            def={unref(getShowFold)}
            disabled={!unref(getShowMultipleTab)}
          />

          <SwitchItem
            title={t('layout.setting.sidebar')}
            event={HandlerEnum.MENU_SHOW_SIDEBAR}
            def={unref(getShowMenu)}
            disabled={unref(getIsHorizontal)}
          />
          <SwitchItem
            title={t('layout.setting.footer')}
            event={HandlerEnum.SHOW_FOOTER}
            def={unref(getShowFooter)}
          />
          <SwitchItem
            title={t('layout.setting.fullContent')}
            event={HandlerEnum.FULL_CONTENT}
            def={unref(getFullContent)}
          />

          <SwitchItem
            title={t('layout.setting.grayMode')}
            event={HandlerEnum.GRAY_MODE}
            def={unref(getGrayMode)}
          />

          <SwitchItem
            title={t('layout.setting.colorWeak')}
            event={HandlerEnum.COLOR_WEAK}
            def={unref(getColorWeak)}
          />
        </>
      );
    }

    return () => (
      <BasicDrawer
        {...attrs}
        title={t('layout.setting.drawerTitle')}
        width={330}
        class="setting-drawer"
      >
        {unref(getShowDarkModeToggle) && <Divider>{() => t('layout.setting.darkMode')}</Divider>}
        {unref(getShowDarkModeToggle) && <AppDarkModeToggle class="mx-auto" />}
        <Divider>{() => t('layout.setting.navMode')}</Divider>
        {renderSidebar()}
        {/* <Divider>{() => t('layout.setting.sysTheme')}</Divider>
        {renderMainTheme()}
        <Divider>{() => t('layout.setting.headerTheme')}</Divider>
        {renderHeaderTheme()} */}
        <Divider>{() => t('layout.setting.sidebarTheme')}</Divider>
        {renderSideBarTheme()}
        <Divider>{() => t('layout.setting.interfaceFunction')}</Divider>
        {renderFeatures()}
        <Divider>{() => t('layout.setting.interfaceDisplay')}</Divider>
        {renderContent()}
        <Divider />
        <SettingFooter />
      </BasicDrawer>
    );
  },
});
