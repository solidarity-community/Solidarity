import { DialogDefault, NotificationHost } from '@3mo/modelx'

export class Service {
	static notifyInfo(...parameters: Parameters<typeof NotificationHost.instance.notifyInfo>) {
		return NotificationHost.instance.notifyInfo(...parameters)
	}

	static notifySuccess(...parameters: Parameters<typeof NotificationHost.instance.notifySuccess>) {
		return NotificationHost.instance.notifySuccess(...parameters)
	}

	static notifyWarning(...parameters: Parameters<typeof NotificationHost.instance.notifyWarning>) {
		return NotificationHost.instance.notifyWarning(...parameters)
	}

	static notifyError(...parameters: Parameters<typeof NotificationHost.instance.notifyError>) {
		return NotificationHost.instance.notifyError(...parameters)
	}

	static throwAndNotify(errorOrErrorMessage: Error | string) {
		this.notifyError(typeof errorOrErrorMessage === 'string' ? errorOrErrorMessage : errorOrErrorMessage.message)
		throw typeof errorOrErrorMessage === 'string' ? new Error(errorOrErrorMessage) : errorOrErrorMessage
	}

	static confirmDeletion(handleDeletion: () => Promise<void>, parameters?: { heading?: string, content?: string }) {
		return new DialogDefault({
			heading: parameters?.heading || 'Confirm Deletion',
			content: parameters?.content || 'Are you sure you want to delete this irreversibly?',
			primaryButtonText: 'Delete',
			primaryButtonAction: () => handleDeletion(),
			secondaryButtonText: 'Cancel',
		}).confirm()
	}
}