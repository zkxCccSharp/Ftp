import type { AppRouteModule } from '@/router/types';

import { getParentLayout, LAYOUT } from '@/router/constant';
import { t } from '@/hooks/web/useI18n';

const comp: AppRouteModule = {
  path: '/comp',
  name: 'Comp',
  component: LAYOUT,
  redirect: '/comp/basic',
  meta: {
    orderNo: 30,
    icon: 'ion:layers-outline',
    title: t('routes.demo.comp.comp'),
  },

  children: [
    {
      path: 'exTask',
      name: 'exTask',
      component: () => import('@/views/demo/table/ExTask.vue'),
      meta: {
        title: t('routes.demo.table.ExTask'),
      },
    },
    {
      path: 'queryTask',
      name: 'queryTask',
      component: () => import('@/views/demo/table/QueryTask.vue'),
      meta: {
        title: t('routes.demo.table.QueryTask'),
      },
    },
    {
      path: 'uploadTask',
      name: 'uploadTask',
      component: () => import('@/views/demo/form/UseForm.vue'),
      meta: {
        title: t('routes.demo.table.uploadTask'),
      },
    },
    {
      path: 'downloadTask',
      name: 'downloadTask',
      component: () => import('@/views/demo/table/EditRowTable.vue'),
      meta: {
        title: t('routes.demo.table.downloadTask'),
      },
    },
    {
      path: 'allTask',
      name: 'allTask',
      component: () => import('@/views/demo/table/AllTask.vue'),
      meta: {
        title: t('routes.demo.table.allTask'),
      },
    },
    {
      path: 'table',
      name: 'TableDemo',
      redirect: '/comp/table/basic',
      component: getParentLayout('TableDemo'),
      meta: {
        // icon: 'carbon:table-split',
        title: t('routes.demo.table.table'),
      },

      children: [
        {
          path: 'basic',
          name: 'ListBasicPage',
          component: () => import('@/views/demo/page/list/basic/index.vue'),
          meta: {
            title: t('routes.demo.page.listBasic'),
          },
        },
        {
          path: 'basic',
          name: 'TableBasicDemo',
          component: () => import('@/views/demo/table/Basic.vue'),
          meta: {
            title: t('routes.demo.table.basic'),
          },
        },
        {
          path: 'treeTable',
          name: 'TreeTableDemo',
          component: () => import('@/views/demo/table/TreeTable.vue'),
          meta: {
            title: t('routes.demo.table.treeTable'),
          },
        },
        {
          path: 'fetchTable',
          name: 'FetchTableDemo',
          component: () => import('@/views/demo/table/FetchTable.vue'),
          meta: {
            title: t('routes.demo.table.fetchTable'),
          },
        },
        {
          path: 'fixedColumn',
          name: 'FixedColumnDemo',
          component: () => import('@/views/demo/table/FixedColumn.vue'),
          meta: {
            title: t('routes.demo.table.fixedColumn'),
          },
        },
        {
          path: 'customerCell',
          name: 'CustomerCellDemo',
          component: () => import('@/views/demo/table/CustomerCell.vue'),
          meta: {
            title: t('routes.demo.table.customerCell'),
          },
        },
        {
          path: 'formTable',
          name: 'FormTableDemo',
          component: () => import('@/views/demo/table/FormTable.vue'),
          meta: {
            title: t('routes.demo.table.formTable'),
          },
        },
        {
          path: 'useTable',
          name: 'UseTableDemo',
          component: () => import('@/views/demo/table/UseTable.vue'),
          meta: {
            title: t('routes.demo.table.useTable'),
          },
        },
        {
          path: 'refTable',
          name: 'RefTableDemo',
          component: () => import('@/views/demo/table/RefTable.vue'),
          meta: {
            title: t('routes.demo.table.refTable'),
          },
        },
        {
          path: 'multipleHeader',
          name: 'MultipleHeaderDemo',
          component: () => import('@/views/demo/table/MultipleHeader.vue'),
          meta: {
            title: t('routes.demo.table.multipleHeader'),
          },
        },
        {
          path: 'mergeHeader',
          name: 'MergeHeaderDemo',
          component: () => import('@/views/demo/table/MergeHeader.vue'),
          meta: {
            title: t('routes.demo.table.mergeHeader'),
          },
        },
        {
          path: 'expandTable',
          name: 'ExpandTableDemo',
          component: () => import('@/views/demo/table/ExpandTable.vue'),
          meta: {
            title: t('routes.demo.table.expandTable'),
          },
        },
        {
          path: 'fixedHeight',
          name: 'FixedHeightDemo',
          component: () => import('@/views/demo/table/FixedHeight.vue'),
          meta: {
            title: t('routes.demo.table.fixedHeight'),
          },
        },
        {
          path: 'footerTable',
          name: 'FooterTableDemo',
          component: () => import('@/views/demo/table/FooterTable.vue'),
          meta: {
            title: t('routes.demo.table.footerTable'),
          },
        },
        {
          path: 'editCellTable',
          name: 'EditCellTableDemo',
          component: () => import('@/views/demo/table/EditCellTable.vue'),
          meta: {
            title: t('routes.demo.table.editCellTable'),
          },
        },
        {
          path: 'editRowTable',
          name: 'EditRowTableDemo',
          component: () => import('@/views/demo/table/EditRowTable.vue'),
          meta: {
            title: t('routes.demo.table.editRowTable'),
          },
        },
        {
          path: 'authColumn',
          name: 'AuthColumnDemo',
          component: () => import('@/views/demo/table/AuthColumn.vue'),
          meta: {
            title: t('routes.demo.table.authColumn'),
          },
        },
        {
          path: 'resizeParentHeightTable',
          name: 'ResizeParentHeightTable',
          component: () => import('@/views/demo/table/ResizeParentHeightTable.vue'),
          meta: {
            title: t('routes.demo.table.resizeParentHeightTable'),
          },
        },
        {
          path: 'vxeTable',
          name: 'VxeTableDemo',
          component: () => import('@/views/demo/table/VxeTable.vue'),
          meta: {
            title: t('routes.demo.table.vxeTable'),
          },
        },
      ],
    },
  ],
};

export default comp;
