<template>
  <div class="p-4">
    <div class="flex items-center mb-4">
      <a-input
        v-model="query"
        type="text"
        placeholder="请输入查询条件"
        style="margin-right: 10px"
      />
      <a-button type="primary" @click="fetchTask">查询任务</a-button>
      <a-button type="primary" @click="toggleCanResize" style="margin-left: 10px">
        {{ !canResize ? '自适应高度' : '取消自适应' }}
      </a-button>
    </div>
    <BasicTable
      title="任务列表"
      titleHelpMessage="全部任务"
      :columns="columns"
      :dataSource="data"
      :canResize="canResize"
      :loading="loading"
      :striped="striped"
      :actionColumn="actionColumns"
      :bordered="border"
      showTableSetting
      :pagination="pagination"
      @columns-change="handleColumnChange"
      @edit-change="onEditChange"
    >
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'action'">
          <TableAction :actions="createActions(record)" />
        </template>
      </template>
    </BasicTable>
  </div>
</template>

<script lang="ts" setup>
  import { ref, h } from 'vue';
  import axios from 'axios';
  import {
    BasicTable,
    useTable,
    ColumnChangeParam,
    TableAction,
    BasicColumn,
    ActionItem,
    EditRecordRow,
  } from '@/components/Table';
  import { useMessage } from '@/hooks/web/useMessage';
  import { Progress } from 'ant-design-vue';
  import { getAppEnvConfig } from '@/utils/env';

  const { VITE_GLOB_API_URL_FTP } = getAppEnvConfig();

  const { createMessage: msg } = useMessage();
  const isFetching = ref(false);
  const canResize = ref(false);
  const loading = ref(false);
  const striped = ref(true);
  const border = ref(true);
  const pagination = ref<any>(false);
  const data = ref([]);
  const query = ref('');

  const actionColumns = {
    width: 160,
    title: 'Action',
    dataIndex: 'action',
  };

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
    {
      title: '任务进度',
      dataIndex: 'RemoteIpPort',
      edit: true,
      editRule: true,
      width: 200,
      editComponentProps: () => {
        return {
          max: 100,
          min: 0,
        };
      },
      editRender: ({ text }) => {
        return h(Progress, { percent: Number(text) });
      },
    },
  ];

  async function fetchTask() {
    try {
      if (isFetching.value) {
        return;
      }
      msg.loading({ content: '正在查询...', duration: 0, key: 'querying' });
      isFetching.value = true;
      loading.value = true;
      const requestBody = {
        zkx: query.value,
      };
      const response = await axios.post(VITE_GLOB_API_URL_FTP + '/Task', requestBody);
      data.value = response.data;
      pagination.value = { pageSize: 15 };
      loading.value = false;
      isFetching.value = false;
      msg.success({ content: '查询成功', key: 'querying' });
    } catch (error) {
      console.error('Error fetching data from API:', error);
      msg.error({ content: '查询失败', key: 'querying' });
    }
  }

  function createActions(record: EditRecordRow): ActionItem[] {
    return [
      {
        label: '编辑',
        onClick: () => handleEdit(record),
        disabled: record.RemoteIpPort >= 100, // Disable the button if progress is 100 or more
      },
    ];
  }

  async function handleEdit(record: EditRecordRow) {
    try {
      const response = await axios.post(VITE_GLOB_API_URL_FTP + '/RetryTask', { id: record.Id });
      const newProgress = response.data;

      // Update the record's progress
      const index = data.value.findIndex((item: any) => item.Id === record.Id);
      if (index !== -1) {
        data.value[index].RemoteIpPort = newProgress;
      }

      msg.success({ content: '任务重试成功' });
    } catch (error) {
      console.error('Error retrying task:', error);
      msg.error({ content: '任务重试失败' });
    }
  }

  function toggleCanResize() {
    canResize.value = !canResize.value;
  }

  function handleColumnChange(data: ColumnChangeParam[]) {
    console.log('ColumnChanged', data);
  }

  function onEditChange(record: EditRecordRow) {
    console.log('EditChange', record);
  }
</script>
