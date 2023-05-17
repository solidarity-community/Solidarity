import { NotificationHost } from '@a11d/lit-application'
import { DialogDefault } from '@3mo/standard-dialogs'

export class Service {
	private static get notificationHostInstance() {
		return NotificationHost.instance as NotificationHost
	}

	static notifyInfo(...parameters: Parameters<typeof Service.notificationHostInstance.notifyInfo>) {
		return Service.notificationHostInstance.notifyInfo(...parameters)
	}

	static notifySuccess(...parameters: Parameters<typeof Service.notificationHostInstance.notifySuccess>) {
		return Service.notificationHostInstance.notifySuccess(...parameters)
	}

	static notifyWarning(...parameters: Parameters<typeof Service.notificationHostInstance.notifyWarning>) {
		return Service.notificationHostInstance.notifyWarning(...parameters)
	}

	static notifyError(...parameters: Parameters<typeof Service.notificationHostInstance.notifyError>) {
		return Service.notificationHostInstance.notifyError(...parameters)
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
			primaryAction: () => handleDeletion(),
			secondaryButtonText: 'Cancel',
		}).confirm()
	}
}