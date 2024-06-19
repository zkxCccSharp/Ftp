<template>
  <div class="p-4">
    <BasicTable @register="registerTable" @edit-change="onEditChange">
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'action'">
          <TableAction :actions="createActions(record)" />
        </template>
      </template>
    </BasicTable>
  </div>
</template>
<script lang="ts" setup>
  import { ref } from 'vue';
  import {
    BasicTable,
    useTable,
    TableAction,
    BasicColumn,
    ActionItem,
    EditRecordRow,
  } from '@/components/Table';
  import { optionsListApi } from '@/api/demo/select';

  import { demoListApi } from '@/api/demo/table';
  import { treeOptionsListApi } from '@/api/demo/tree';
  import { cloneDeep } from 'lodash-es';
  import { useMessage } from '@/hooks/web/useMessage';

  const columns: BasicColumn[] = [
    {
      title: 'ID',
      dataIndex: 'Id',
      fixed: 'left',
      width: 150,
    },
    {
      title: '状态',
      dataIndex: 'State',
      width: 100,
      filters: [
        { text: 'Start', value: 'Start' },
        { text: 'Done', value: 'Done' },
        { text: 'Exception', value: 'Exception' },
      ],
    },
    {
      title: '操作类型',
      width: 140,
      dataIndex: 'OperType',
      filters: [
        { text: 'DownloadDirectory', value: 'DownloadDirectory' },
        { text: 'DownloadFile', value: 'DownloadFile' },
        { text: 'UploadDirectory', value: 'UploadDirectory' },
        { text: 'UploadFile', value: 'UploadFile' },
      ],
    },
    {
      title: '创建时间',
      width: 150,
      sorter: true,
      dataIndex: 'CreateTime',
    },
    {
      title: '开始时间',
      dataIndex: 'StartTime',
      width: 150,
      sorter: true,
    },
    {
      title: '结束时间',
      width: 150,
      sorter: true,
      dataIndex: 'DoneTime',
    },
    {
      title: '服务器配置',
      width: 150,
      dataIndex: 'RemoteIpPort',
    },
    {
      title: '操作参数',
      dataIndex: 'OperParam',
    },
    {
      title: '错误信息',
      width: 150,
      dataIndex: 'ErrorMsg',
      defaultHidden: true,
    },
    {
      title: '原始任务',
      width: 150,
      sorter: true,
      dataIndex: 'AncestorTaskId',
      defaultHidden: true,
    },
    {
      title: '会话Id',
      width: 150,
      sorter: true,
      dataIndex: 'SessionId',
      defaultHidden: true,
    },
  ];

  const { createMessage: msg } = useMessage();
  const currentEditKeyRef = ref('');
  const [registerTable] = useTable({
    title: '任务列表',
    titleHelpMessage: ['全部任务'],
    api: demoListApi,
    columns: columns,
    showIndexColumn: false,
    showTableSetting: true,
    tableSetting: { fullScreen: true },
    actionColumn: {
      width: 160,
      title: 'Action',
      dataIndex: 'action',
    },
  });

  function handleEdit(record: EditRecordRow) {
    currentEditKeyRef.value = record.key;
    record.onEdit?.(true);
  }

  function handleCancel(record: EditRecordRow) {
    currentEditKeyRef.value = '';
    record.onEdit?.(false, false);
  }

  async function handleSave(record: EditRecordRow) {
    // 校验
    msg.loading({ content: '正在保存...', duration: 0, key: 'saving' });
    const valid = await record.onValid?.();
    if (valid) {
      try {
        const data = cloneDeep(record.editValueRefs);
        console.log(data);
        //TODO 此处将数据提交给服务器保存
        // ...
        // 保存之后提交编辑状态
        const pass = await record.onEdit?.(false, true);
        if (pass) {
          currentEditKeyRef.value = '';
        }
        msg.success({ content: '数据已保存', key: 'saving' });
      } catch (error) {
        msg.error({ content: '保存失败', key: 'saving' });
      }
    } else {
      msg.error({ content: '请填写正确的数据', key: 'saving' });
    }
  }

  function createActions(record: EditRecordRow): ActionItem[] {
    if (!record.editable) {
      return [
        {
          label: '编辑',
          disabled: currentEditKeyRef.value ? currentEditKeyRef.value !== record.key : false,
          onClick: handleEdit.bind(null, record),
        },
      ];
    }
    return [
      {
        label: '保存',
        onClick: handleSave.bind(null, record),
      },
      {
        label: '取消',
        popConfirm: {
          title: '是否取消编辑',
          confirm: handleCancel.bind(null, record),
        },
      },
    ];
  }

  function onEditChange({ column, value, record }) {
    // 本例
    if (column.dataIndex === 'id') {
      record.editValueRefs.name4.value = `${value}`;
    }
    console.log(column, value, record);
  }
</script>
